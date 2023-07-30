using AvSearch;
using AvSearch.Hooks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Giact.Steps
{
    [Binding]
    public class GOFACSteps : CommonObjects
    {
        private readonly string _gofacUrl;
        private readonly string _orderUrl;
        RestClient _client;
        IRestResponse _response;
        RestRequest _request;
        public string _sourceFileId = Helpers.GetSourceFileId(20);

        private readonly ScenarioContext _scenarioContext;

        public GOFACSteps(ScenarioContext scenarioContext)
        {
            var configuration = new TestConfiguration().GetConfiguration();
            scenarioContext.Set(configuration, "Configuration");

            configuration = scenarioContext.Get<IConfiguration>("Configuration");
            _gofacUrl = configuration["OFACUrl"];
            _orderUrl = configuration["OrderUrl"];
            _client = new RestClient(_gofacUrl);
            _response = new RestResponse();

        }

        [When(@"I create and post gOFAC inquiry for (.*)")]
        public void WhenICreateAndPostGOFACInquiryForparty(string party)
        {
            JObject jObject = new();    
            jObject = CreatefOFACQuery(party);
            GlobalObjects.JsonObject = jObject;
            GlobalObjects.QueryObject = jObject.ToString();

            _request = new RestRequest("/inquiry", Method.POST);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            _request.AddJsonBody(GlobalObjects.QueryObject);
            _response = _client.Execute(_request);
            Assert.That(_response.IsSuccessful);
        }

        [Then(@"I verify response schema according to (.*)")]
        public void ThenIVerifyResponseSchemaAccordingToParty(string party)
        {
            JObject jObject = JObject.Parse(_response.Content);
            Assert.NotNull(jObject.SelectToken("responses")[0].SelectToken("detail").SelectToken("success"));
            Assert.NotNull(jObject.SelectToken("responses")[0].SelectToken("detail").SelectToken("itemReferenceId"));
            Assert.NotNull(jObject.SelectToken("responses")[0].SelectToken("detail").SelectToken("createdDate"));
            Assert.NotNull(jObject.SelectToken("responses")[0].SelectToken("errorDetail"));
            Assert.NotNull(jObject.SelectToken("responses")[0].SelectToken("verificationResponse"));
            Assert.NotNull(jObject.SelectToken("responses")[0].SelectToken("ofacListPotentialMatches"));
            if (party.Equals("individual"))
            {
                Assert.AreEqual(GlobalObjects.JsonObject.SelectToken("individual").SelectToken("firstName").ToString(), jObject.SelectToken("responses")[0].SelectToken("request").SelectToken("individual").SelectToken("firstName").ToString());
                Assert.AreEqual(GlobalObjects.JsonObject.SelectToken("individual").SelectToken("lastName").ToString(), jObject.SelectToken("responses")[0].SelectToken("request").SelectToken("individual").SelectToken("lastName").ToString());
                Assert.IsEmpty(jObject.SelectToken("responses")[0].SelectToken("request").SelectToken("business"));
            }
            else if (party.Equals("business"))
            {
                Assert.AreEqual(GlobalObjects.JsonObject.SelectToken("business").SelectToken("businessName").ToString(), jObject.SelectToken("responses")[0].SelectToken("request").SelectToken("business").SelectToken("businessName").ToString());
                Assert.IsEmpty(jObject.SelectToken("responses")[0].SelectToken("request").SelectToken("individual"));
            }
        }


        [Then(@"I can assert that Order Record created")]
        public void ThenICanAssertThatOrderRecordCreated()
        {
            JObject responseObj = JObject.Parse(_response.Content);
            GlobalObjects.OrderId = responseObj.SelectToken("orderId").ToString();
            _client = new RestClient(_orderUrl);

            _request = new RestRequest($"/order/{GlobalObjects.OrderId}", Method.GET);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            _response = _client.Execute(_request);
            Assert.That(_response.IsSuccessful);
            responseObj = JObject.Parse(_response.Content);
            Assert.AreEqual("Complete", responseObj.SelectToken("services")[0].SelectToken("status").ToString());
            Assert.AreEqual("InquiryService.QueryOfac", responseObj.SelectToken("services")[0].SelectToken("serviceName").ToString());
            Assert.NotNull(responseObj.SelectToken("caller").SelectToken("obo").SelectToken("companyId"));
        }

        [When(@"I create and post gOFAC inquiry with missing (.*)")]
        public void WhenICreateAndPostGOFACInquiryWithMissingField(string field)
        {
            JObject jObject = new();

            if (field == "firstName")
            {
                jObject = CreatefOFACQuery("individual");
                jObject.SelectToken("individual").SelectToken("firstName").Replace("");
            }
            else if (field == "lastName")
            {
                jObject = CreatefOFACQuery("individual");
                jObject.SelectToken("individual").SelectToken("lastName").Replace("");
            }
            else if (field == "businessName") {
                jObject = CreatefOFACQuery("business");
                jObject.SelectToken("business").SelectToken("businessName").Replace("");
            }
            GlobalObjects.QueryObject = jObject.ToString();
            _request = new RestRequest("/inquiry", Method.POST);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            _request.AddJsonBody(GlobalObjects.QueryObject);
            _response = _client.Execute(_request);            
        }
               
        [Then(@"I can assert on failing response according to missing (.*)")]
        public void ThenICanAssertOnFailingResponseAccordingToMissingField(string field)
        {
            Assert.AreEqual(400, (int)_response.StatusCode);
            if (field == "firstName")
                Assert.That(_response.Content.Contains("Individual -> First Name name must not be empty"));              
            else if (field == "lastName")
                Assert.That(_response.Content.Contains("Individual -> Last Name name must not be empty"));
            else if (field == "businessName")
                Assert.That(_response.Content.Contains("Business -> Business Name must not be empty"));
        }

        [When(@"I create and post gOFAC inquiry")]
        public void WhenICreateAndPostGOFACInquiry()
        {
            JObject jObject = new();
            jObject = CreatefOFACQuery("individual");
            GlobalObjects.QueryObject = jObject.ToString();

            _request = new RestRequest("/inquiry", Method.POST);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            _request.AddJsonBody(GlobalObjects.QueryObject);
            _response = _client.Execute(_request);            
        }

        [Then(@"I can assert that authorization required")]
        public void ThenICanAssertThatAuthorizationRequired()
        {
            Assert.AreEqual(401, (int)_response.StatusCode);
        }

    }
}
