using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Utils;
using AvSearch.Hooks;
using AvSearch.Resources;
using Gherkin;
using Gherkin.CucumberMessages.Types;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using System.Xml.Linq;
using TechTalk.SpecFlow;

namespace AvSearch.Steps.ApiSteps;

[Binding]
internal class ImportSteps
{
    private readonly string _authUrl;
    private readonly string _avUrl;
    public RestClient _avClient;
    public RestClient _authClient;
    public RestRequest _request;
    public IRestResponse _response;

    public ImportSteps(ScenarioContext scenarioContext)
    {
        var configuration = new TestConfiguration().GetConfiguration();
        scenarioContext.Set(configuration, "Configuration");
        configuration = scenarioContext.Get<IConfiguration>("Configuration");

        _authUrl = configuration["AuthUrl"];
        _avUrl = configuration["AvSearchUrl"];
        _avClient = new RestClient(_avUrl);
        _authClient = new RestClient(_authUrl);
        _response = new RestResponse();

        GlobalObjects.RoutingNumber = "123456789";
        GlobalObjects.AccountNumber = "10987654320";
        GlobalObjects.AccountHolderName = "Santa Clause";
    }

    [When(@"I import to AV data")]
    public void WhenIImportToAVData()
    {
        var avData = new AvData()
        {
            Source = "Test",
            SourceApprovalDate = DateTime.Today,
            Status = "Approved",
            RoutingNumber = GlobalObjects.RoutingNumber,
            AccountNumber = GlobalObjects.AccountNumber,
            BankName = "Acme Bank",
            AccountHolder = new() { name = "Santa Clause" },
            Additional = new { }
        };

        _request = CreateRequest("sm-av-import/v1/account", Method.POST);
        _request.AddJsonBody(avData);
        _response = _avClient.Execute(_request);

        GlobalObjects.AvAccountId = JsonConvert.DeserializeObject<string>(_response.Content);
        Helpers.WaitForSpecificSeconds(1);
    }

    [Given(@"I import to AV data")]
    public void GivenIImportToAVData()
    {
       var avData = new AvData()
        {
            Source = "Test",
            SourceApprovalDate = DateTime.Today,
            Status = "Approved",
            RoutingNumber = GlobalObjects.RoutingNumber,
            AccountNumber = GlobalObjects.AccountNumber,
            BankName = "Acme Bank",
            AccountHolder = new() { name = "Santa Clause" },
            Additional = new { }
        };

        _request = CreateRequest("sm-av-import/v1/account", Method.POST);
        _request.AddJsonBody(avData);
        _response = _avClient.Execute(_request);

        GlobalObjects.AvAccountId = JsonConvert.DeserializeObject<string>(_response.Content);
        Helpers.WaitForSpecificSeconds(1);
    }

    [When(@"I post Ort AV data to import")]
    public void WhenIPostOrtAVDataToImport()
    {
        var ortDatas = new List<OrtData>
        {
            new()
            {
                TimesUsed = "10",
                AbaNumber = "123456789",
                AccountNumber = "10987654321",
                BankName = "Acme Bank",
                Beneficiary = "Some other dude"
            }
        };

        _request = CreateRequest("sm-av-import/v1/account/import/ort", Method.POST);
        _request.AddJsonBody(ortDatas);
        _response = _avClient.Execute(_request);
        GlobalObjects.ResponseObject = _response.Content;
        var jsonArray = JArray.Parse(_response.Content);
        GlobalObjects.AccountNumber = jsonArray.First.SelectToken("data").SelectToken("accountNumber").ToString();
        GlobalObjects.RoutingNumber = jsonArray.First.SelectToken("data").SelectToken("abaNumber").ToString();


    }

    [Then(@"I can assert on the response")]
    public void ThenICanAssertOnTheResponse()
    {
        Assert.IsTrue(_response.IsSuccessful);
        GlobalObjects.ResponseObject = _response.Content;
    }

   
    static RestRequest CreateRequest(string url, Method method)
    {
        var request = new RestRequest(url, method);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
        return request;

    }


    [Then(@"I can assert that the entity is removed from the AVS repo")]
    public void ThenICanAssertThatTheEntityIsRemovedFromTheAVSRepo()
    {
        JArray jArray = JArray.Parse(GlobalObjects.ResponseObject);
        Assert.That(jArray.ToString().Contains("[]"));
    }


    [Then(@"I get the approved entity to assert its in the AVS repo")]
    public void ThenIGetTheApprovedEntityToAssertItsInTheAvRepo()
    {
        JArray jArray = JArray.Parse(GlobalObjects.ResponseObject);
        Assert.That(jArray[0].SelectToken("bankName").ToString().Contains("WELLS FARGO BANK NA (ARIZONA)"));
        Assert.That(jArray[0].SelectToken("accountHolder.name").ToString().Contains("Manhattan Elite Prep"));
    }

    [When(@"I perform accounts sync")]
    public void WhenIPerformAccountsSync()
    {
        _request = CreateRequest("sm-av-import/v1/account/sync", Method.POST);
        _response = _avClient.Execute(_request);
    }

    [When(@"I can get account by id")]
    public void WhenICanGetAccountById()
    {
        _request = CreateRequest($"sm-av-import/v1/account/{GlobalObjects.AvAccountId}", Method.GET);
        _response = _avClient.Execute(_request);
        GlobalObjects.ResponseObject = _response.Content;
    }

    [Then(@"I delete account")]
    public void WhenIDeleteAccount()
    {
        Helpers.WaitForSpecificSeconds(1);
        _request = CreateRequest($"sm-av-import/v1/account/{GlobalObjects.AvAccountId}", Method.DELETE);
        _response = _avClient.Execute(_request);
        Assert.AreEqual(204, (int)_response.StatusCode);
    }

    [Then(@"I can assert that account not in AV database")]
    public void ThenICanAssertThatAccountNotInAVDatabase()
    {
        _request = CreateRequest($"sm-av-import/v1/account/{GlobalObjects.AvAccountId}", Method.GET);
        _response = _avClient.Execute(_request);
        Assert.AreEqual(404, (int)_response.StatusCode);
        JObject jObject = JObject.Parse(_response.Content);
        string errorMsg = jObject.SelectToken("error").ToString();
        Assert.AreEqual($"Could not find account with id: {GlobalObjects.AvAccountId}", errorMsg);
    }

    [Then(@"I perform account search")]
    public void ThenIPerformAccountSearch()
    {
        _request = CreateRequest($"sm-av-import/v1/account/routing/{GlobalObjects.RoutingNumber}/account/{GlobalObjects.AccountNumber}", Method.GET);
        _response = _avClient.Execute(_request);
        GlobalObjects.ResponseObject = _response.Content;
        try
        {
            JArray jArray = JArray.Parse(_response.Content);
            GlobalObjects.AvAccountId = jArray[0].SelectToken("id").ToString();
        }
        catch
        {
            Console.WriteLine("Invalid account number or routing number");
        }
    }

    [When(@"I perform account update")]
    public void WhenIPerformAccountUpdate()
    {
        Assert.IsTrue(_response.IsSuccessful, "Account has not been created");
        var avData = new AvData()
        {
            Source = "Test",
            SourceApprovalDate = DateTime.Today,
            Status = "Pending",
            RoutingNumber = "Because value is encrypted it Will be ignored",
            AccountNumber = "Because value is encrypted it Will be ignored",
            BankName = "Citizens Bank",
            AccountHolder = new() { name = "Because value is encrypted it Will be ignored" },
            Additional = new { }
        };

        _request = CreateRequest($"sm-av-import/v1/account/{GlobalObjects.AvAccountId}", Method.PUT);
        _request.AddJsonBody(avData);
        _response = _avClient.Execute(_request);
        Assert.AreEqual(204, (int)_response.StatusCode);
    }

    [Then(@"I assert that update is successful")]
    public void ThenIAssertThatUpdateIsSuccessful()
    {
        JObject jObject= JObject.Parse(GlobalObjects.ResponseObject);
        Assert.AreEqual("Pending", jObject.SelectToken("status").ToString(), "Status was not updated");
        Assert.AreEqual("Citizens Bank", jObject.SelectToken("bankName").ToString(), "Bankm name was not updated");       
    }

    [Then(@"I run the Health Check")]
    public void ThenIRunTheHealthCheck()
    {
        _request = CreateRequest("sm-av-import/v1/health", Method.GET);
        _response = _avClient.Execute(_request);
        Assert.IsTrue(_response.IsSuccessful);
        Assert.AreEqual("Microservice in good health", JsonConvert.DeserializeObject<string>(_response.Content));
    }

    [Then(@"I can get accounts paginated")]
    public void ThenICanGetAccountsPaginated()
    {
        _request = CreateRequest("sm-av-import/v1/account", Method.GET);
        _response = _avClient.Execute(_request);
        Assert.IsTrue(_response.IsSuccessful);
        GlobalObjects.ResponseObject = _response.Content;
    }

    [Then(@"I can assert that account listed in response body")]
    public void ThenICanAssertThatAccountListedInResponseBody()
    {
        JObject jObject = JObject.Parse(GlobalObjects.ResponseObject);
        var accNum = (int)jObject.SelectToken("count");
        Assert.IsTrue(accNum >= 1);
        List<string> accountIds = new List<string>();
        for (int i = 0; i < accNum; i++)
        {
            accountIds.Add(jObject.SelectToken("data")[i].SelectToken("id").ToString());            
        }
        Assert.IsTrue(accountIds.Contains(GlobalObjects.AvAccountId));
    }

    [When(@"I run test with invalid (.*)")]
    public void WhenIRunTestWithInvalid(string id)
    {
         GlobalObjects.AvAccountId = id;
    }

    [Then(@"I can assert on the response according to (.*)")]
    public void ThenICanAssertOnTheResponseAccordingTo(string id)
    {
        JObject jObject = JObject.Parse(GlobalObjects.ResponseObject);
        if (id == "")
        {
            Assert.IsTrue(_response.IsSuccessful);
        }
        else
        {
            Assert.AreEqual(404, (int)_response.StatusCode);
            Assert.That(jObject.SelectToken("error").ToString().Contains($"Could not find account with id: {id}"));
        }
    }

    [When(@"I run test using invalid (.*) and (.*)")]
    public void WhenIRunTestUsingInvalidAnd(string routingNum, string accountNum)
    {
        GlobalObjects.RoutingNumber = routingNum;
        GlobalObjects.AccountNumber = accountNum;
    }

    [Then(@"I can assert on the response based on (.*) and (.*)")]
    public void ThenICanAssertOnTheResponseBasedOnAnd(string routingNum, string accountNum)
    {
        Assert.AreEqual(404, (int)_response.StatusCode);

        if (routingNum == "" || accountNum == "") 
        {
            Assert.IsEmpty(GlobalObjects.ResponseObject);
        }
        else
        {
            JObject jObject = JObject.Parse(GlobalObjects.ResponseObject);
            Assert.That(jObject.SelectToken("operationId").ToString().Contains("SearchAccounts"));
            Assert.That(jObject.SelectToken("error").ToString().Contains($"Could not find account with routing/account: {routingNum}/{accountNum}"));
        }
    }

    [When(@"I import to AV data with invalid (.*) at (.*)")]
    public void WhenIImportToAVDataWithInvalidValueAtToken(string value, string token)
    {
        var avData = new AvData()
        {
            Source = "Test",
            SourceApprovalDate = DateTime.Today,
            Status = "Approved",
            RoutingNumber = GlobalObjects.RoutingNumber,
            AccountNumber = GlobalObjects.AccountNumber,
            BankName = "Acme Bank",
            AccountHolder = new() { name = "Santa Clause" },
            Additional = new { }
        };
        switch(token){
            case "source":
                avData.Source = value;
                break;
            case "status":
                avData.Status = value;
                break;
            case "routingNumber":
                avData.RoutingNumber = value;
                break;
            case "accountNumber":
                avData.AccountNumber = value;
                break;
            case "bankName":
                avData.BankName = value;
                break;
            case "accountHolder":
                avData.AccountHolder.name = value;
                break;
        }
        _request = CreateRequest("sm-av-import/v1/account", Method.POST);
        _request.AddJsonBody(avData);
        _response = _avClient.Execute(_request);        
    }

    [Then(@"I can assert on the response by (.*) with (.*)")]
    public void ThenICanAssertOnTheResponseByToken(string token, int statusCode)
    {
        JObject jObject;
        Assert.AreEqual(statusCode, (int)_response.StatusCode);
        switch (token)
        {
            case "source":                
                jObject = JObject.Parse(_response.Content);
                Assert.That(jObject.SelectToken("errors")[0].ToString().Contains("Source field required."));
                break;
            case "status":
                jObject = JObject.Parse(_response.Content);
                Assert.That(jObject.SelectToken("errors")[0].ToString().Contains("Error converting value"));
                break;
            case "routingNumber":
                jObject = JObject.Parse(_response.Content);
                Assert.That(jObject.SelectToken("errors")[0].ToString().Contains("RoutingNumber field required."));
                break;
            case "accountNumber":
                jObject = JObject.Parse(_response.Content);
                Assert.That(jObject.SelectToken("errors")[0].ToString().Contains("AccountNumber field required."));
                break;
            case "bankName":
                GlobalObjects.AvAccountId = JsonConvert.DeserializeObject<string>(_response.Content);
                break;
            case "accountHolder":
                GlobalObjects.AvAccountId = JsonConvert.DeserializeObject<string>(_response.Content);
                break;
        }
        if ((int)_response.StatusCode == 201)
        {
            _request = CreateRequest($"sm-av-import/v1/account/{GlobalObjects.AvAccountId}", Method.DELETE);
            _response = _avClient.Execute(_request);
        }

    }

    [When(@"I perform account update with invalid (.*) at (.*)")]
    public void WhenIPerformAccountUpdateWithInvalidAtSource(string value, string token)
    {
        var avData = new AvData()
        {
            Source = "Test",
            SourceApprovalDate = DateTime.Today,
            Status = "Pending",
            RoutingNumber = "Because value is encrypted it Will be ignored",
            AccountNumber = "Because value is encrypted it Will be ignored",
            BankName = "Citizens Bank",
            AccountHolder = new() { name = "Because value is encrypted it Will be ignored" },
            Additional = new { }
        };
        switch (token)
        {
            case "source":
                avData.Source = value;
                break;
            case "status":
                avData.Status = value;
                break;
            case "routingNumber":
                avData.RoutingNumber = value;
                break;
            case "accountNumber":
                avData.AccountNumber = value;
                break;
            case "bankName":
                avData.BankName = value;
                break;
            case "accountHolder":
                avData.AccountHolder.name = value;
                break;
        }
        _request = CreateRequest($"sm-av-import/v1/account/{GlobalObjects.AvAccountId}", Method.PUT);
        _request.AddJsonBody(avData);
        _response = _avClient.Execute(_request);
    }
   
    [Then(@"I delete account and assert based on (.*) and (.*)")]
    public void ThenIDeleteAccountAndAssertBasedOnAnd(string id, int statusCode)
    {
        _request = CreateRequest($"sm-av-import/v1/account/{id}", Method.DELETE);
        _response = _avClient.Execute(_request);
        Assert.AreEqual(statusCode, (int)_response.StatusCode);
    }

    [When(@"I post Ort AV data with invalid (.*) at (.*)")]
    public void WhenIPostOrtAVDataWithInvalidAtTimesUSed(string value, string token)
    {
        var ortDatas = new List<OrtData>
        {
            new()
            {
                TimesUsed = "10",
                AbaNumber = "123456789",
                AccountNumber = "10987654321",
                BankName = "Acme Bank",
                Beneficiary = "Some other dude"
            }
        };
        switch (token)
        {
            case "timesUsed":
                ortDatas[0].TimesUsed = value;
                break;
            case "abaNumber":
                ortDatas[0].AbaNumber = value;
                break;
            case "accountNumber":
                ortDatas[0].AccountNumber = value;
                break;
            case "bankName":
                ortDatas[0].BankName = value;
                break;
            case "beneficiary":
                ortDatas[0].Beneficiary = value;
                break;
           
        }
        _request = CreateRequest("sm-av-import/v1/account/import/ort", Method.POST);
        _request.AddJsonBody(ortDatas);
        _response = _avClient.Execute(_request);
    }

    [Then(@"I can assert on response by (.*) with (.*)")]
    public void ThenICanAssertOnResponseByTokenWithValue(string token, string value)
    {
        string errorMsg = "";
        switch (token)
        {            
            case "abaNumber":
                errorMsg = "AbaNumber field required.";
                break;
            case "accountNumber":
                errorMsg = "AccountNumber field required.";
                break;            
            case "beneficiary":
                errorMsg = "Beneficiary field required.";
                break;           
        }
        if (!(token == "timesUsed" || token == "bankName") && value.IsNullOrEmpty())
        {
            JObject jObject = JObject.Parse(_response.Content);
            Assert.That(jObject.SelectToken("errors")[0].ToString().Contains(errorMsg));
            Assert.AreEqual(400, (int)_response.StatusCode);
        }
        else
        {
            Assert.AreEqual(200, (int)_response.StatusCode);
            var jsonArray = JArray.Parse(_response.Content);
            GlobalObjects.AccountNumber = jsonArray.First.SelectToken("data").SelectToken("accountNumber").ToString();
            GlobalObjects.RoutingNumber = jsonArray.First.SelectToken("data").SelectToken("abaNumber").ToString();
        }
        
    }

}
