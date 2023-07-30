using AventStack.ExtentReports.Utils;
using AvSearch.Hooks;
using AvSearch.Resources;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using TechTalk.SpecFlow;

namespace AvSearch.Steps
{
    [Binding]
    public class NameParserSteps
    {
        private readonly string _avUrl;
        private readonly string _orderUrl;

        RestClient _avClient;
        RestClient _orderClient;
        IRestResponse _response;
        RestRequest _request;
        public NameParserSteps(ScenarioContext scenarioContext)
        {
            var configuration = new TestConfiguration().GetConfiguration();
            scenarioContext.Set(configuration, "Configuration");
            configuration = scenarioContext.Get<IConfiguration>("Configuration");
            _avUrl = configuration["AvSearchUrl"];
            _orderUrl = configuration["OrderUrl"];
            _avClient = new RestClient(_avUrl);
            _orderClient = new RestClient(_orderUrl);
            _response = new RestResponse();
        }

        [When(@"I pass in array of (.*)")]
        public void WhenIPassInArrayOfNames(string name)
        {
            _request = new RestRequest($"sm-parser/v1/names", Method.POST);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            var jObject = new List<object>(); ;
            jObject.Add(name);
            _request.AddJsonBody(jObject);
            _response = _avClient.Execute(_request);
            Assert.That(_response.IsSuccessful);
            GlobalObjects.ResponseObject = _response.Content;
        }
        [Then(@"I expect the names to be separeted into Individual or Entity and has expected (.*) of objects")]
        public void ThenIExpectTheNamesToBeSeparetedIntoIndividualOrEntityAndHasExpectedOfObjects(int amount)
        {         
            JArray jArray = JArray.Parse(GlobalObjects.ResponseObject);

            var results = jArray[0].SelectToken("results");

            for (int i = 0; i < results.Count(); i++)
            {
                switch (results[i].SelectToken("type").ToString()){
                    case "Individual":
                        Assert.NotNull(results[i].SelectToken("first"));
                        Assert.NotNull(results[i].SelectToken("last"));
                        break;
                    case "Entity":
                        Assert.NotNull(results[i].SelectToken("business"));
                        break;                     
                }
            }
            Assert.AreEqual(amount, results.Count(), "Number of objects not matching");

        }

    }
}