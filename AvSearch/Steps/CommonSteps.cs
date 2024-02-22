using AvSearch.Pages;
using AvSearch.Drivers;
using AvSearch.Hooks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using RestSharp;
using TechTalk.SpecFlow;
using AvSearch.Hooks;
using AvSearch.Resources;

namespace AvSearch.Steps
{
    [Binding]
    public class CommonSteps
    {
        //************************************************************************************************************
        private IWebDriver _driver;
        private readonly IConfiguration _configuration;
        public ScenarioContext _scenarioContext;
        private string _avUrl;
        public RestClient _avClient;
        public RestClient _client;
        public RestRequest _request;
        public IRestResponse _response;
        private LoginPage _loginPage;
        private readonly string _authUrl;
        private readonly string _baseUrl;

        public CommonSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _configuration = scenarioContext.Get<IConfiguration>("Configuration");
            var Configuration = new TestConfiguration().GetConfiguration();
            _avUrl = Configuration.GetSection("AvSearchUrl").Value;
            _authUrl = Configuration["AuthUrl"];
            _avClient = new RestClient(_avUrl);
            _request = new RestRequest();
            _response = new RestResponse();
        }

        [Given(@"I get access token for Account Verification")]
        public void GivenIGetTheAccessTokenForESign()
        {
            //Creating Client connection
            _client = new RestClient(_authUrl);
            //Creating request to get data from api
            _request = new RestRequest("/realms/supermarket/token", Method.POST);
            _request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            _request.AddParameter("username", "test@test.com");
            _request.AddParameter("password", "test123");
            _request.AddParameter("grant_type", "password");
            _request.AddParameter("client_id", "sm-portal");

            Console.WriteLine("Request -->>>>" + _request.ToString());
            //Executing request to api and checking server response
            _response = _client.Execute(_request);
            Assert.AreEqual(200, (int)_response.StatusCode, "Status code is not 200");

            //Parse the JSON response
            var JsonObject = JObject.Parse(_response.Content);
            GlobalObjects.AccessToken = JsonObject.GetValue("access_token").ToString();
            Console.WriteLine("Access Token -->>>>" + GlobalObjects.AccessToken);
        }

        [Given(@"I login to av import")]
        public void GivenILoginToAvImport()
        {
            _loginPage.Login(GlobalObjects.UserName = "test@test.com", GlobalObjects.Password = "test123");

        }
     

        [Given(@"I navigate to the av import ui")]
        public void GivenINavigateToTheAvImportUi()
        {
            _driver = _scenarioContext.Get<SeleniumDriver>("SeleniumDriver").Setup("Chrome");
            _driver.Url = Path.Combine(_configuration["AvImportUiUrl"]);
            _loginPage = new LoginPage(_driver);
        }

        [Given(@"AV database is empty")]
        public void GivenAVDatabaseIsEmpty()
        {
            _request = new RestRequest($"sm-av-import/v1/account", Method.GET);
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            
            _response = _avClient.Execute(_request);
            Assert.That(_response.IsSuccessful);
            GlobalObjects.ResponseObject = _response.Content;
            JObject jObject = JObject.Parse(_response.Content);
            var accNum = jObject.SelectToken("count");
            for (int i = 0; i < (int)accNum; i++)
            {
                string account = jObject.SelectToken("data")[i].SelectToken("id").ToString();
                _request = new RestRequest($"sm-av-import/v1/account/{account}", Method.DELETE);
                _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
                _response = _avClient.Execute(_request);

                Assert.That(_response.IsSuccessful);
            }
        }


    }
}