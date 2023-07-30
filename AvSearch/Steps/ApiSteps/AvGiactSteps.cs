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
    public class AvGiactSteps
    {
        private readonly string _avUrl;
        private readonly string _orderUrl;

        RestClient _avClient;
        RestClient _orderClient;
        IRestResponse _response;
        RestRequest _request;

        private readonly ScenarioContext _scenarioContext;

        public AvGiactSteps(ScenarioContext scenarioContext)
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

        [Given(@"I perform a search via AV giact")]
        public void GivenIPerformASearchViaAVGiact()
        {
            var avRequest = new List<object>();
            var check = new Checks() { CheckNumber = "1000" };
            var checks = new List<Checks> {check };
            Smi smiObject = new Smi(); 
            Requests requestObject = new Requests();
            Account account = new ();
            var document = new List<Document>(); 
            account = new Account() { RoutingNumber = Helpers.RandomDigits(9), AccountNumber = Helpers.RandomDigits(16), Checks = checks };
            Party party = new ();
            Entity entity = new Entity() { FullName = Faker.Name.FullName() };
            TaxIdentifier taxIdentifier = new TaxIdentifier() {Value = "1232", Type = "ssn" };
            UserInfo userInfo = new UserInfo();
            userInfo = new UserInfo() { Name = Faker.Name.First(), Email = Faker.Internet.Email(), Phone = Faker.Phone.Number() };
            CompanyInfo companyInfo = new CompanyInfo();
            companyInfo = new CompanyInfo() { CompanyName = Faker.Company.Name(), Address = Faker.Address.StreetAddress(), Email = Faker.Internet.Email(), Phone = Faker.Phone.Number() };
            
            party = new Party() 
            {
                Entity = entity,
                TaxIdentifier = taxIdentifier 

            };
            smiObject = new Smi()
            {
                SourceFileId = Helpers.RandomString(25),
                Transaction = Helpers.RandomDigits(25),
                UserInfo = userInfo,
                CompanyInfo = companyInfo
            };
            requestObject = new Requests()
            {
                Party = party,
                Account = account,
                Documents = document
            };
            avRequest.Add(requestObject);
            var inquiryObject = new {smi = smiObject, request = avRequest};
            GlobalObjects.AvInquiryJson = JsonConvert.SerializeObject(inquiryObject, Formatting.None);
            _request = new RestRequest($"avs/v2/inquiry", Method.POST);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            _request.AddJsonBody(GlobalObjects.AvInquiryJson);
            _response = _avClient.Execute(_request);
            GlobalObjects.ResponseObject = _response.Content;
            JObject jObject = JObject.Parse(_response.Content);
            GlobalObjects.OrderId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("orderId");
            GlobalObjects.ServiceId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("serviceId").ToString();
            GlobalObjects.AccountNumber = account.AccountNumber;
            GlobalObjects.RoutingNumber = account.RoutingNumber;
        }

        [When(@"I perform an entity search via AV giact")]
        public void WhenIPerformAnEntitySearchViaAVGiact()
        {
            InquiryObject.entitySearch();

            _request = new RestRequest($"avs/v1/inquiry", Method.POST);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            _request.AddJsonBody(GlobalObjects.AvInquiryJson);
            _response = _avClient.Execute(_request);
            GlobalObjects.ResponseObject = _response.Content;
            JObject jObject = JObject.Parse(_response.Content);
            GlobalObjects.OrderId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("orderId");
            GlobalObjects.ServiceId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("serviceId").ToString();
        }

        [When(@"I perform a Giact search")]
        public void WhenIPerformAGiactSearch()
        {
            JObject entitySearch = InquiryObject.entitySearch();
            entitySearch["request"][0]["party"]["entity"]["fullName"].Replace(GlobalObjects.AccountHolderName);
            entitySearch["request"][0]["account"]["routingNumber"].Replace(GlobalObjects.RoutingNumber);
            entitySearch["request"][0]["account"]["accountNumber"].Replace(GlobalObjects.AccountNumber);
            entitySearch["request"][0]["account"]["checks"][0]["checkNumber"].Replace("099");
        


            GlobalObjects.entityName = entitySearch["request"][0]["party"]["entity"]["fullName"].ToString();

            _request = new RestRequest($"avs/v2/inquiry", Method.POST);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            _request.AddJsonBody(entitySearch.ToString());
            _response = _avClient.Execute(_request);
            Assert.That(_response.IsSuccessful);

            GlobalObjects.ResponseObject = _response.Content;
            JObject jObject = JObject.Parse(_response.Content);
            GlobalObjects.OrderId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("orderId").ToString();
            GlobalObjects.ServiceId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("serviceId").ToString();
        }

        [When(@"I get queue by orderId")]
        public void WhenIGetQueueByOrderId()
        {
            _request = new RestRequest($"sm-av-import/v1/account/{GlobalObjects.OrderId}", Method.GET);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            _response = _avClient.Execute(_request);
            Assert.That(_response.IsSuccessful);
            GlobalObjects.ResponseObject = _response.Content;
        }

        [Then(@"I can assert that its been added to the queue")]
        public void ThenICanAssertThatItsBeenAddedToTheQueue()
        {
            JArray jArray = JArray.Parse(GlobalObjects.ResponseObject);

            Assert.That(jArray.LastOrDefault().SelectToken("status").ToString().Contains("Pending"));

            foreach (JObject jOb in jArray)
            {
                Assert.That(jOb.ToString().Contains("status"));
                Assert.That(jOb.ToString().Contains("id"));
                Assert.That(jOb.ToString().Contains("createDate"));
                Assert.That(jOb.ToString().Contains("updateDate"));
                Assert.That(jOb.ToString().Contains("sourceApprovalDate"));
                Assert.That(jOb.ToString().Contains("source"));
                Assert.That(jOb.ToString().Contains("routingNumber"));
                Assert.That(jOb.ToString().Contains("accountNumber"));
                Assert.That(jOb.ToString().Contains("bankName"));
                Assert.That(jOb.ToString().Contains("additional"));
                Assert.That(jOb.SelectToken("accountHolder").ToString().Contains("name"));
                Assert.That(jOb.SelectToken("accountHolder").ToString().Contains("taxId"));
                Assert.That(jOb.SelectToken("accountHolder").ToString().Contains("dob"));
                Assert.That(jOb.SelectToken("accountHolder").ToString().Contains("phone"));
                Assert.That(jOb.SelectToken("accountHolder").ToString().Contains("address"));
                Assert.That(jOb.SelectToken("accountHolder").ToString().Contains("driversLicense"));
            }
        }

        [When(@"I verify AV data was imported")]
        public void WhenIVerifyAVDataWasImported()
        {
           // await Task.Delay(1000);
            _request = CreateRequest($"sm-av-search/v1/search/routing/{GlobalObjects.RoutingNumber}/account/{GlobalObjects.AccountNumber}", Method.GET);
            _response = _avClient.Execute(_request);
            Assert.That(_response.IsSuccessful);        
            var jArray = JArray.Parse(_response.Content);
            Assert.IsTrue(jArray?.Any(x => x.SelectToken("id").ToString() == GlobalObjects.AvAccountId));
        }


        [Given(@"I perform a giact search")]
        public void GivenIPerformAGiactSearch()
        {
            JObject entitySearch = InquiryObject.entitySearch();
            entitySearch["request"][0]["party"]["entity"]["fullName"].Replace("Manhattan Elite Prep");
            entitySearch["request"][0]["account"]["routingNumber"].Replace("122105278");
            entitySearch["request"][0]["account"]["accountNumber"].Replace("0000000018");
            entitySearch["request"][0]["account"]["checks"][0]["checkNumber"].Replace("099");

            GlobalObjects.entityName = entitySearch["request"][0]["party"]["entity"]["fullName"].ToString();
            GlobalObjects.RoutingNumber = entitySearch["request"][0]["account"]["routingNumber"].ToString();
            GlobalObjects.AccountNumber = entitySearch["request"][0]["account"]["accountNumber"].ToString();
            GlobalObjects.AccountHolderName = entitySearch["request"][0]["party"]["entity"]["fullName"].ToString();
            _request = new RestRequest($"avs/v2/inquiry", Method.POST);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            _request.AddJsonBody(entitySearch.ToString());
            _response = _avClient.Execute(_request);
            Assert.That(_response.IsSuccessful);

            GlobalObjects.ResponseObject = _response.Content;
            JObject jObject = JObject.Parse(_response.Content);
            GlobalObjects.orderId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("orderId");
            GlobalObjects.OrderId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("orderId");
            GlobalObjects.ServiceId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("serviceId").ToString();

        }

        [Given(@"I perform an (.*) a giact search")]
        public void GivenIPerformAnAGiactSearch(string source)
        {
            JObject entitySearch = InquiryObject.indivudalSearch();

            if (source == "verifiedSource")
            {
                entitySearch = InquiryObject.indivudalSearchWithDoc();
            }
            else if(source == "nonVerifiedSource")
            {
                entitySearch["request"][0]["party"]["individual"]["name"]["first"].Replace(Faker.Name.First());
                entitySearch["request"][0]["party"]["individual"]["name"]["last"].Replace(Faker.Name.Last());
                entitySearch["request"][0]["party"]["taxIdentifier"]["value"].Replace(Helpers.RandomDigits(4));
                entitySearch["request"][0]["account"]["routingNumber"].Replace("123456789");
                entitySearch["request"][0]["account"]["accountNumber"].Replace(Helpers.RandomDigits(9));
                entitySearch["request"][0]["account"]["checks"][0]["checkNumber"].Replace(Helpers.RandomDigits(4));
          

            }
            else if(source == "verifiedAdvancedSource")
            {
            }
            else if(source == "nonVerifiedAdvancedSource")
            {
                entitySearch["request"][0]["party"]["individual"]["name"]["first"].Replace(Faker.Name.First());
                entitySearch["request"][0]["party"]["individual"]["name"]["last"].Replace(Faker.Name.Last());
                entitySearch["request"][0]["party"]["taxIdentifier"]["value"].Replace(Helpers.RandomDigits(4));
                entitySearch["request"][0]["account"]["routingNumber"].Replace("123456789");
                entitySearch["request"][0]["account"]["accountNumber"].Replace(Helpers.RandomDigits(9));
                entitySearch["request"][0]["account"]["checks"][0]["checkNumber"].Replace(Helpers.RandomDigits(4));
            }



            string firstName =  entitySearch["request"][0]["party"]["individual"]["name"]["first"].ToString();
            string lastName =  entitySearch["request"][0]["party"]["individual"]["name"]["last"].ToString();
            GlobalObjects.entityName = $"{firstName} {lastName}";
            GlobalObjects.RoutingNumber = entitySearch["request"][0]["account"]["routingNumber"].ToString();
            GlobalObjects.AccountNumber = entitySearch["request"][0]["account"]["accountNumber"].ToString();

            _request = new RestRequest($"avs/v2/inquiry", Method.POST);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            _request.AddJsonBody(entitySearch.ToString());
            _response = _avClient.Execute(_request);
            Assert.That(_response.IsSuccessful);

            GlobalObjects.ResponseObject = _response.Content;
            JObject jObject = JObject.Parse(_response.Content);
            GlobalObjects.orderId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("orderId");
            GlobalObjects.OrderId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("orderId");
            GlobalObjects.ServiceId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("serviceId").ToString();

        }

        [Given(@"I perform a individual a giact search")]
        public void GivenIPerformAIndividualAGiactSearch()
        {
            JObject individualSearch = InquiryObject.indivudalSearch();

            _request = new RestRequest($"avs/v2/inquiry", Method.POST);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            _request.AddJsonBody(individualSearch.ToString());
            _response = _avClient.Execute(_request);
            Assert.That(_response.IsSuccessful);

            GlobalObjects.ResponseObject = _response.Content;
            JObject jObject = JObject.Parse(_response.Content);
            GlobalObjects.orderId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("orderId");
            GlobalObjects.OrderId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("orderId");
            GlobalObjects.ServiceId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("serviceId").ToString();
        }


        [Given(@"I perform a entity (.*) a giact search")]
        public void GivenIPerformAEntityAGiactSearch(string type)
        {
            JObject entitySearch = InquiryObject.entitySearch();

            if(type == "basicEntityVerified")
            {
                entitySearch["request"][0]["party"]["entity"]["fullName"].Replace("Manhattan Elite Prep");
                entitySearch["request"][0]["account"]["routingNumber"].Replace("122105278");
                entitySearch["request"][0]["account"]["accountNumber"].Replace("0000000018");
                entitySearch["request"][0]["account"]["checks"][0]["checkNumber"].Replace("099");             
                //entitySearch["request"][0]["account"]["checks"][0]["checkNumber"].Remove();             

            }
            else if (type == "basicEntityNonVerified")
            {
                entitySearch["request"][0]["party"]["entity"]["fullName"].Replace(Faker.Name.First());              
                entitySearch["request"][0]["party"]["taxIdentifier"]["value"].Replace(Helpers.RandomDigits(4));
                entitySearch["request"][0]["account"]["routingNumber"].Replace("123456789");
                entitySearch["request"][0]["account"]["accountNumber"].Replace(Helpers.RandomDigits(9));
                entitySearch["request"][0]["account"]["checks"][0]["checkNumber"].Replace("099");             

            }
            else if (type == "advancedEntityNonVerified")
            {
                entitySearch["request"][0]["party"]["entity"]["fullName"].Replace(Faker.Name.First());
                entitySearch["request"][0]["party"]["taxIdentifier"]["value"].Replace(Helpers.RandomDigits(4));
                entitySearch["request"][0]["party"]["taxIdentifier"]["type"].Replace("ein");
                entitySearch["request"][0]["account"]["routingNumber"].Replace("123456789");
                entitySearch["request"][0]["account"]["accountNumber"].Replace(Helpers.RandomDigits(9));
                entitySearch["request"][0]["account"]["checks"][0]["checkNumber"].Replace(Helpers.RandomDigits(4));
            }
            else if (type == "advancedEntityVerified")
            {
                entitySearch["request"][0]["party"]["entity"]["fullName"].Replace("Manhattan Elite Prep");
                entitySearch["request"][0]["account"]["routingNumber"].Replace("122105278");
                entitySearch["request"][0]["account"]["accountNumber"].Replace("0000000018");
                entitySearch["request"][0]["party"]["taxIdentifier"]["value"].Replace("1919");
                entitySearch["request"][0]["party"]["taxIdentifier"]["type"].Replace("ein");
                entitySearch["request"][0]["account"]["checks"][0]["checkNumber"].Replace("19");
            }

            GlobalObjects.entityName = entitySearch["request"][0]["party"]["entity"]["fullName"].ToString();
            GlobalObjects.RoutingNumber = entitySearch["request"][0]["account"]["routingNumber"].ToString();
            GlobalObjects.AccountNumber = entitySearch["request"][0]["account"]["accountNumber"].ToString();

            _request = new RestRequest($"avs/v2/inquiry", Method.POST);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            _request.AddJsonBody(entitySearch.ToString());
            _response = _avClient.Execute(_request);
            Assert.That(_response.IsSuccessful);

            GlobalObjects.ResponseObject = _response.Content;
            JObject jObject = JObject.Parse(_response.Content);
            GlobalObjects.orderId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("orderId");
            GlobalObjects.OrderId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("orderId");
            GlobalObjects.ServiceId = jObject.SelectToken("response[0]").SelectToken("response").SelectToken("serviceId").ToString();

        }



        [When(@"I get the approved account")]
        public void WhenIGetTheApprovedAccount()
        {
            _request = new RestRequest($"sm-av-import/v1/account/routing/{"122105278"}/account/{"0000000018"}", Method.GET);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            //_request.AddParameter("application/json", GlobalObjects.ResponseObject, ParameterType.RequestBody);
            _response = _avClient.Execute(_request);
            Assert.That(_response.IsSuccessful);

           JArray jObject = JArray.Parse(_response.Content);
            GlobalObjects.ResponseObject = _response.Content;
            GlobalObjects.orderId = jObject[0].SelectToken("id").ToString();
        }


        [When(@"I update the source approval date for over ninety days")]
        public void IUpdateTheSourceApprovalDateForOverNinetyDays()
        {
             JArray jArray = JArray.Parse(GlobalObjects.ResponseObject);
            JObject jObject = JObject.Parse(jArray[0].ToString());
            var sourceDate = jArray[0].SelectToken("sourceApprovalDate").ToString();

            //jArray[0].SelectToken("sourceApprovalDate").ToString().Replace(sourceDate, "2023-10-13T14:09:34.838318-05:00");
            //jArray[0]["sourceApprovalDate"] = "2024-10-13T14:09:34.838318-05:00";
            jObject["sourceApprovalDate"] = "2034-10-13T14:09:34.838318-05:00";
            //var body = ImportObject.PostQueue();
            _request = new RestRequest($"sm-av-import/v1/account/{GlobalObjects.orderId}", Method.PUT);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            _request.AddParameter("application/json", jObject.ToString(), ParameterType.RequestBody);
            //_request.AddJsonBody(jObject);
            _response = _avClient.Execute(_request);
            Assert.That(_response.IsSuccessful);
            GlobalObjects.ResponseObject = _response.Content;
        }

        [Then(@"I assert that the account approval date is up to date")]
        public void ThenIAssertThatTheAccountApprovalDateIsUpToDate()
        {
            _request = new RestRequest($"sm-av-import/v1/account/routing/{"122105278"}/account/{"0000000018"}", Method.GET);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            //_request.AddParameter("application/json", GlobalObjects.ResponseObject, ParameterType.RequestBody);
            _response = _avClient.Execute(_request);
            Assert.That(_response.IsSuccessful);

            JArray jObject = JArray.Parse(_response.Content);
            var updateDate = jObject[0]["updateDate"].ToString();
            var nowDateTime = DateTime.Now;
            DateTime convertedDate = DateTime.Parse(updateDate);
            Assert.That(convertedDate <= nowDateTime);

            //Assert on the approval date
            GlobalObjects.ResponseObject = _response.Content;
            GlobalObjects.orderId = jObject[0].SelectToken("id").ToString();
        }

        //[When(@"I verify AV data was imported")]
        //public async void WhenIVerifyAVDataWasImported()
        //{
        //    await Task.Delay(1000);

        //    _request = CreateRequest($"sm-av-search/v1/search/routing/{GlobalObjects.RoutingNumber}/account/{GlobalObjects.AccountNumber}", Method.GET);
        //    _response = _avClient.Execute(_request);
        //}

        [When(@"I post a new queue")]
        public void WhenIPostANewQueue()
        {
            // JObject jObject = JObject.Parse(ImportObject.PostQueue);
            var body = ImportObject.PostQueue();
             _request = new RestRequest($"sm-av-import/v1/account", Method.POST);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            _request.AddParameter("application/json", GlobalObjects.ResponseObject, ParameterType.RequestBody);
            //_request.AddJsonBody(GlobalObjects.ResponseObject);
            _response = _avClient.Execute(_request);
            Assert.That(_response.IsSuccessful);
            GlobalObjects.ResponseObject = _response.Content;
        }

        [When(@"I get the queue by routing and account number")]
        public void WhenIGetTheQueueByRoutingAndAccountNumber()
        {       
            _request = new RestRequest($"sm-av-import/v1/account/routing/{"123456789"}/account/{"10987654321"}", Method.GET);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            //_request.AddParameter("application/json", GlobalObjects.ResponseObject, ParameterType.RequestBody);   
            _response = _avClient.Execute(_request);
            Assert.That(_response.IsSuccessful);
            GlobalObjects.ResponseObject = _response.Content;
        }



        [When(@"I invoke a get by orderId")]
        public void WhenIInvokeAGetByOrderId()
        {

            var jArray = JArray.Parse(_response.Content);

            Assert.IsTrue(jArray?.Any(x => x.SelectToken("id").ToString() == GlobalObjects.AvAccountId));
        }

        [Then(@"I can assert the search source type is (.*)")]
        public void ThenICanAssertTheSearchSourceTypeIs(string sourceType)
        {
            Task.Delay(1000);
            _request = CreateRequest($"order/{GlobalObjects.OrderId}", Method.GET);
            _response = _orderClient.Execute(_request);

            Assert.That(_response.IsSuccessful);

            var jObject = JObject.Parse(_response.Content);

            var source = jObject.SelectTokens("services")
                                .SelectMany(x => x)
                                .Where(x => x.SelectToken("id").ToString() == GlobalObjects.ServiceId)
                                .Select(x => x.SelectToken("attributes.source"))
                                .FirstOrDefault()
                                .ToString();

            Assert.AreEqual(sourceType, source);
        }

        [Then(@"I can assert that the search source type is (.*)")]
        public async Task ThenICanAssertThatTheSearchSourceTypeIsAsync(string sourceType)
        {
            await Task.Delay(1000);

            _request = CreateRequest($"order/{GlobalObjects.OrderId}", Method.GET);
            _response = _orderClient.Execute(_request);

            Assert.That(_response.IsSuccessful);

            var jObject = JObject.Parse(_response.Content);



            var source = jObject.SelectTokens("services")
                                .SelectMany(x => x)
                                .Where(x => x.SelectToken("id").ToString() == GlobalObjects.ServiceId)
                                .Select(x => x.SelectToken("attributes.source"))
                                .FirstOrDefault()
                                .ToString();

            Assert.AreEqual(sourceType, source);
        }


        static RestRequest CreateRequest(string url, Method method)
        {
            var request = new RestRequest(url, method);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + GlobalObjects.AccessToken);
            return request;
        }

        [Given(@"I verify AV data record does not exist")]
        public void GivenIVerifyAVDataRecordDoesNotExist()
        {
            _request = CreateRequest($"sm-av-search/v1/search/routing/{Helpers.RandomDigits(9)}/account/{Helpers.RandomDigits(16)}", Method.GET);
            _response = _avClient.Execute(_request);
            Assert.That(_response.IsSuccessful);

            var jArray = JArray.Parse(_response.Content);

            var jObject = jArray.Where(x => x.SelectTokens("accountHolder.name").Contains(GlobalObjects.AccountHolderName))
                                .Select(x => x.SelectToken("id"))
                                .FirstOrDefault();

            if (jObject is not null)
            {
                var accountId = jObject.ToString();

                _request = CreateRequest($"sm-av-import/v1/account/{accountId}", Method.DELETE);
                _response = _avClient.Execute(_request);

                Assert.That(_response.IsSuccessful);
            }
        }

    }
}
