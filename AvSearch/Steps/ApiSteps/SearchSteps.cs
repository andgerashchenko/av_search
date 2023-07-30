using AvSearch.Hooks;
using AvSearch.Hooks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using TechTalk.SpecFlow;

namespace AvSearch.Steps.ApiSteps
{
    [Binding]
    internal class SearchSteps
    {
        private readonly string _authUrl;
        private readonly string _avUrl;
        public RestClient _avClient;
        public RestClient _authClient;
        public RestRequest _request;
        public IRestResponse _response;
        JObject _jObject = new JObject();

        public SearchSteps(ScenarioContext scenarioContext)
        {
            var configuration = new TestConfiguration().GetConfiguration();
            scenarioContext.Set(configuration, "Configuration");

            configuration = scenarioContext.Get<IConfiguration>("Configuration");
            _authUrl = configuration["AuthUrl"];
            _avUrl = configuration["AvSearchUrl"];
            _avClient = new RestClient(_avUrl);
            _authClient = new RestClient(_authUrl);
            _response = new RestResponse();
        }


        [Given(@"Healthchecks for account verification is up")]
        public void GivenHealthchecksForIsUpIn()
        {
            _request = new RestRequest($"sm-av-search/v1/health", Method.GET);
            _response = _avClient.Execute(_request);
            Assert.AreEqual(200, (int)_response.StatusCode, "Status code is not 200");
        }


        [When(@"I perform a search by (.*) and (.*)")]
        public void WhenIPerformASearchByAccountNumberAndRoutingNumber(string accountNumber, string routingNumber)
        {
            _request = new RestRequest($"sm-av-search/v1/search/routing/{routingNumber}/account/{accountNumber}", Method.GET);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            _response = _avClient.Execute(_request);
            Helpers.WaitForSpecificSeconds(1);
            GlobalObjects.ResponseObject = _response.Content;
        }

        [Then(@"I can assert on search response")]
        public void ThenICanAssertOnSearchResponse()
        {
            JArray jArray = JArray.Parse(GlobalObjects.ResponseObject);
            Assert.NotNull(jArray[0].SelectToken("createDate"));
            Assert.NotNull(jArray[0].SelectToken("updateDate"));
            Assert.NotNull(jArray[0].SelectToken("source"));
            Assert.NotNull(jArray[0].SelectToken("routingNumber"));
            Assert.NotNull(jArray[0].SelectToken("accountNumber"));
            Assert.NotNull(jArray[0].SelectToken("bankName"));             
            Assert.NotNull(jArray[0].SelectToken("accountHolder.name"));
            Assert.NotNull(jArray[0].SelectToken("accountHolder.taxId"));
            Assert.NotNull(jArray[0].SelectToken("accountHolder.dob"));
            Assert.NotNull(jArray[0].SelectToken("accountHolder.phone"));
            Assert.NotNull(jArray[0].SelectToken("accountHolder.address"));
            Assert.NotNull(jArray[0].SelectToken("accountHolder.driversLicense"));
        }

        [Then(@"I can assert on invalid search response based on (.*) and (.*)")]
        public void TThenICanAssertOnInvalidSearchResponseBasedOnAnd(string accountNum, string routingNum)
        {            
            if (routingNum == "" || accountNum == "")
            {
                Assert.AreEqual(404, (int)_response.StatusCode);
                Assert.IsEmpty(GlobalObjects.ResponseObject);
            }
            else
            {
                Assert.AreEqual(200, (int)_response.StatusCode);
                JArray jArray = JArray.Parse(GlobalObjects.ResponseObject);
                Assert.IsEmpty(jArray);             
            }
        }
        
    }
}
