using AvSearch;
using AvSearch.Hooks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace Giact.Steps;

[Binding]
public class CommonObjects
{

    public static JObject CreatefOFACQuery()
    {

        JObject queryJson = new();

        var entityObj = new Entity { fullName = Faker.Company.Name() };

        var taxObj = new TaxIdentifier { value = Helpers.RandomDigits(9), type = "ssn" };

        var nameObj = new Name { first = Faker.Name.First(), last = Faker.Name.Last() };

        var partiesObj = new Parties { name = nameObj };

        var listPartiesObj = new List<Parties> { partiesObj };

        var requestObj = new GofacRequest { parties = listPartiesObj };

        var userInfoObj = new UserInfo
        {
            name = Faker.Name.FullName(),
            email = Faker.Internet.Email(),
            phone = Faker.Phone.Number()
        };

        var companyInfoObj = new CompanyInfo
        {
            companyName = Faker.Company.Name(),
            address = Faker.Address.StreetAddress(),
            phone = Faker.Phone.Number(),
            email = Faker.Internet.Email(),
        };

        var smiObj = new Smi
        {
            sourceFileId = Faker.Company.Name(),
            transaction = Faker.Company.Name(),
            userInfo = userInfoObj,
            companyInfo = companyInfoObj
        };

        var queryObj = new
        {
            Smi = smiObj,
            Request = requestObj
        };

        queryJson = JObject.FromObject(queryObj);
        var queryJsonConvert = JsonConvert.SerializeObject(JObject.FromObject(queryObj), Formatting.None);
        GlobalObjects.QueryObject = queryJsonConvert;
        GlobalObjects.Transaction = smiObj.transaction.ToString();


        return queryJson;
    }

    public static JObject CreatefOFACQuery(string party)
    {
        JObject queryJson = new();
        if (party.Equals("individual"))
        {
            var nameObj = new IndividualGofac { firstName = Faker.Name.First(), lastName = Faker.Name.Last() };
            var queryObj = new
            {
                individual = nameObj
            };
        queryJson = JObject.FromObject(queryObj);
        }
        else if (party.Equals("business"))
        {
            var businessObj = new Business { businessName = Faker.Company.Name() };
            var queryObj = new
            {
                business = businessObj
            };            
            queryJson = JObject.FromObject(queryObj);
        }        
        return queryJson;
    }
        public static JObject CreateGiactQuery()
    {
        JObject queryJson = new();

        var documentObj = new Document { fileName = "supportingdoc.png", document = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=" };
        
        var listDocumentObj = new List<Document> { documentObj };

        var checkObj = new Check { checkNumber = Helpers.RandomDigits(4) };

        var listChecksObj = new List<Check> { checkObj };

        var accountObj = new Account
        {
            routingNumber = Helpers.RandomDigits(9),
            accountNumber = Helpers.RandomDigits(9),
            checks = listChecksObj
        };

        var entityObj = new Entity {fullName = Faker.Company.Name()};

        var taxObj = new TaxIdentifier { value = Helpers.RandomDigits(9), type = "ssn" };

        var nameObj = new Name {first = Faker.Name.First(), last = Faker.Name.Last()};

        var individualObj = new Individual {name = nameObj};

        var partyObj = new Party
        {
            individual =individualObj,
            taxIdentifier = taxObj,
            entity = entityObj
        };

        var requestObj = new Request
        {
            party = partyObj,
            account = accountObj,
            documents = listDocumentObj
        };

        var listRequestObj = new List<Request> { requestObj };

        var userInfoObj = new UserInfo
        {
            name = Faker.Name.FullName(),
            email = Faker.Internet.Email(),
            phone = Faker.Phone.Number()
        };

        var companyInfoObj = new CompanyInfo
        {
            companyName = Faker.Company.Name(),
            address = Faker.Address.StreetAddress(),
            phone = Faker.Phone.Number(),
            email = Faker.Internet.Email(),
        };

        var smiObj = new Smi
        {
            sourceFileId = Faker.Company.Name(),
            transaction = Faker.Company.Name(),
            userInfo = userInfoObj,
            companyInfo = companyInfoObj
        };

        var queryObj = new
        {
            Smi = smiObj,
            Request = listRequestObj
        };

        queryJson = JObject.FromObject(queryObj);
        var queryJsonConvert = JsonConvert.SerializeObject(JObject.FromObject(queryObj), Formatting.None);
        GlobalObjects.QueryObject = queryJsonConvert;
        GlobalObjects.Transaction = smiObj.transaction.ToString();        

        return queryJson;
    }    

    public class Email
    {
        public string to { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string salutation { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public bool sendCc { get; set; }
    }

    public class EmailRoot
    {
        public Email email { get; set; }
    }

    public class UserInfo
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }

    public class CompanyInfo
    {
        public string companyName { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
    }

    public class Smi
    {
        public string sourceFileId { get; set; }
        public string transaction { get; set; }
        public UserInfo userInfo { get; set; }
        public CompanyInfo companyInfo { get; set; }
    }

    public class Name
    {
        public string first { get; set; }
        public string last { get; set; }
    }
    public class IndividualGofac
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
    }

    public class Individual
    {
        public Name name { get; set; }
    }
    public class Parties
    {
        public Name name { get; set; }
    }

    public class TaxIdentifier
    {
        public string value { get; set; }
        public string type { get; set; }
    }

    public class Entity
    {
        public string fullName { get; set; }
    }
    public class Business
    {
        public string businessName { get; set; }
    }

    public class Party
    {
        public Individual individual { get; set; }
        public TaxIdentifier taxIdentifier { get; set; }
        public Entity entity { get; set; }
    }

    public class Check
    {
        public string checkNumber { get; set; }
    }

    public class Account
    {
        public string routingNumber { get; set; }
        public string accountNumber { get; set; }
        public List<Check> checks { get; set; }
    }

    public class Document
    {
        public string fileName { get; set; }
        public string document { get; set; }
    }

    public class Request
    {
        public Party party { get; set; }
        public Account account { get; set; }
        public List<Document> documents { get; set; }
    }
    public class GofacRequest
    {        
        public List<Parties> parties { get; set; }
       
    }

    public class Query
    {
        public Smi smi { get; set; }
        public List<Request> request { get; set; }
    }


}
