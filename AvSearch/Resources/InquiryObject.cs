using AvSearch.Hooks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvSearch.Resources
{
    public static class InquiryObject
    {
        public static JObject entitySearch()
        {
            var avRequest = new List<object>();
            var check = new Checks() { CheckNumber = "1000" };
            var checks = new List<Checks> { check };
            Smi smiObject = new Smi();
            Requests requestObject = new Requests();
            Account account = new();           
            var documents = new List<Document>() {  };
            account = new Account() { RoutingNumber = "123456789", AccountNumber = "10987654321", Checks = checks };
            Party party = new();
            Entity entity = new Entity() { FullName = "John Cena" };
            TaxIdentifier taxIdentifier = new TaxIdentifier() { Value = "1232", Type = "ssn" };
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
                Documents = documents
            };
            avRequest.Add(requestObject);
            var inquiryObject = new { smi = smiObject, request = avRequest };
            GlobalObjects.AvInquiryJson = JsonConvert.SerializeObject(inquiryObject, Formatting.None);
            
            return JObject.FromObject(inquiryObject);
        }

        public static JObject indivudalSearch()
        {
            var avRequest = new List<object>();
            var check = new Checks() { /*CheckNumber = "0567"*/ };
            var checks = new List<Checks> { check };
            Smi smiObject = new Smi();
            IndividualRequests requestObject = new IndividualRequests();
            Account account = new();
            var document = new Document() { /*fileName = "supportingdoc.png", document = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII="*/ };
            var documents = new List<Document>() {/*document*/ };
            account = new Account() { RoutingNumber = "122105278", AccountNumber = "0000000022", Checks = checks };
            IndividualParty indivParty = new();
            Name name = new Name() { first = "Jane", last = "Smith" };
            Individual individualParty = new Individual() { name = name};
            TaxIdentifier taxIdentifier = new TaxIdentifier() { Value = "123567779", Type = "ssn" };
            UserInfo userInfo = new UserInfo();
            userInfo = new UserInfo() { Name = Faker.Name.First(), Email = Faker.Internet.Email(), Phone = Faker.Phone.Number() };
            CompanyInfo companyInfo = new CompanyInfo();
            companyInfo = new CompanyInfo() { CompanyName = Faker.Company.Name(), Address = Faker.Address.StreetAddress(), Email = Faker.Internet.Email(), Phone = Faker.Phone.Number() };

            indivParty = new IndividualParty()
            {
                individual = individualParty,
                TaxIdentifier = taxIdentifier

            };

            smiObject = new Smi()
            {
                SourceFileId = Helpers.RandomString(25),
                UserInfo = userInfo,
                Transaction = Helpers.RandomDigits(25),
                CompanyInfo = companyInfo
            };

            requestObject = new IndividualRequests()
            {
                party = indivParty,
                account = account,
                documents = documents
            };
            avRequest.Add(requestObject);
            var inquiryObject = new { smi = smiObject, request = avRequest };
            GlobalObjects.AvInquiryJson = JsonConvert.SerializeObject(inquiryObject, Formatting.None);

            return JObject.FromObject(inquiryObject);
        }

        public static Smi smi => new()
        {
            SourceFileId = "Test",
            Transaction = "123456789",
            UserInfo = new(),
            CompanyInfo = new()
        };        
        public static Requests requests => new()
        {
            Party = new(),
            Account = new Account(),
            Documents = new()

        };



        public static JObject indivudalSearchWithDoc()
        {
            var avRequest = new List<object>();
            var check = new Checks() { /*CheckNumber = "0567"*/ };
            var checks = new List<Checks> { check };
            Smi smiObject = new Smi();
            IndividualRequests requestObject = new IndividualRequests();
            Account account = new();
            var document = new Document() { fileName = "supportingdoc.png", document = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=" };
            var documents = new List<Document>() { document };
            account = new Account() { RoutingNumber = "122105278", AccountNumber = "0000000022", Checks = checks };
            IndividualParty indivParty = new();
            Name name = new Name() { first = "Jane", last = "Smith" };
            Individual individualParty = new Individual() { name = name };
            TaxIdentifier taxIdentifier = new TaxIdentifier() { Value = "123567779", Type = "ssn" };
            UserInfo userInfo = new UserInfo();
            userInfo = new UserInfo() { Name = Faker.Name.First(), Email = Faker.Internet.Email(), Phone = Faker.Phone.Number() };
            CompanyInfo companyInfo = new CompanyInfo();
            companyInfo = new CompanyInfo() { CompanyName = Faker.Company.Name(), Address = Faker.Address.StreetAddress(), Email = Faker.Internet.Email(), Phone = Faker.Phone.Number() };

            indivParty = new IndividualParty()
            {
                individual = individualParty,
                TaxIdentifier = taxIdentifier

            };

            smiObject = new Smi()
            {
                SourceFileId = Helpers.RandomString(25),
                UserInfo = userInfo,
                Transaction = Helpers.RandomDigits(25),
                CompanyInfo = companyInfo
            };

            requestObject = new IndividualRequests()
            {
                party = indivParty,
                account = account,
                documents = documents
            };
            avRequest.Add(requestObject);
            var inquiryObject = new { smi = smiObject, request = avRequest };
            GlobalObjects.AvInquiryJson = JsonConvert.SerializeObject(inquiryObject, Formatting.None);

            return JObject.FromObject(inquiryObject);
        }

    }


    public class Smi 
    {
        [JsonProperty("sourceFileId")]
        public string SourceFileId { get; set; }         
        [JsonProperty("transaction")]
        public string Transaction { get; set; }
        [JsonProperty("userInfo")]
        public UserInfo UserInfo { get; set; }          
        [JsonProperty("companyInfo")]
        public CompanyInfo CompanyInfo { get; set; }    
    }
    public class CompanyInfo
    {
        [JsonProperty("companyName")]
        public string CompanyName { get; set;}
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
    }


    public class UserInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }            
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
    
    }    
    public class Requests 
    {
        [JsonProperty("party")]
        public Party Party { get; set; }        
        
        [JsonProperty("account")]
        public Account Account { get; set; }        
        
        [JsonProperty("documents")]
        public List<Document> Documents { get; set; }
    }

    public class IndividualRequests
    {
       public IndividualParty party { get; set; }

        [JsonProperty("account")]
        public Account account { get; set; }

        [JsonProperty("documents")]
        public List<Document> documents { get; set; }
    }

    public class Party
    {
        [JsonProperty("entity")]
        public Entity Entity { get; set; }
        [JsonProperty("taxIdentifier")]
        public TaxIdentifier TaxIdentifier { get; set; }
    }

    public class IndividualParty
    {
        public Individual individual { get; set; }

        [JsonProperty("taxIdentifier")]
        public TaxIdentifier TaxIdentifier { get; set; }
    }

    public class TaxIdentifier
    {
        [JsonProperty("value")]
        public string Value { get; set; }        
        [JsonProperty("type")]
        public string Type { get; set; }
    }    
    public class Entity
    {
        [JsonProperty("fullName")]
        public string FullName{ get; set; }
    }
    public partial class Account
    {
        [JsonProperty("routingNumber")]
        public string RoutingNumber { get; set; }

        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("checks")]
        public List<Checks> Checks { get; set; }
    }
    public partial class Checks
    {
        [JsonProperty("checkNumber")]
        public string CheckNumber { get; set; }
    }    

    public class Documents
    {
        [JsonProperty("documents")]
        public List<Document> documents { get; set; }
    }

    public class Document
    {
        public string fileName { get; set; }
        public string document { get; set; }

    }

    public class Individual
    {
        public Name name { get; set; }
    }

    public class Name
    {
        public string first { get; set; }
        public string last { get; set; }
    }

}
