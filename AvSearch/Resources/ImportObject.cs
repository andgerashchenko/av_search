using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AvSearch.Resources
{
    public static class ImportObject
    {
        public static AvData avData => new()
        {
            Source = "Test",
            RoutingNumber = "123456789",
            AccountNumber = "10987654321",
            BankName = "Acme Bank",
            AccountHolder = new(),
            Additional = new(),
        };
        public static OrtData ortData => new()
        {
            TimesUsed = "10",
            AbaNumber = "123456789",
            AccountNumber = "10987654321",
            BankName = "Acme Bank",
            Beneficiary = "Some other dude"
        };

        public static JObject PostQueue()
        {
            JObject queryJson = new();

            var queueData = new AvData
            {
                Source = "Test",
                RoutingNumber = "122105278",
                AccountNumber = "0000000022",
                BankName = "Acme Bank",
                AccountHolder = new() { name = "TestMasters" },
                Additional = new(),
                SourceApprovalDate = new DateTime(),
                Status = "Pending"
              
            };
            //var listOfQueueData = new List<AvData> { queueData };
            queryJson = JObject.FromObject(queueData);
            var queryJsonConvert = JsonConvert.SerializeObject(JObject.FromObject(queueData), Formatting.None);
            GlobalObjects.ResponseObject = queryJsonConvert;
            return queryJson;

        }


    }
}
public class AvData
{
    [JsonProperty("source")]
    public string Source { get; set; }    
    [JsonProperty("routingNumber")]
    public string RoutingNumber { get; set; }
    [JsonProperty("accountNumber")]
    public string AccountNumber { get; set; }
    [JsonProperty("bankName")]
    public string BankName { get; set; }
    [JsonProperty("accountHolder")]
    public accountHolder AccountHolder { get; set; }
    [JsonProperty("additional")]
    public object Additional { get; set; }
    [JsonProperty("status")]
    public string Status{ get; set; }

    [JsonProperty("sourceApprovalDate")]
    public DateTime SourceApprovalDate { get; set; }
}
public class accountHolder
{ 
    public string name { get; set; }
}
public class Additional
{
}
public class OrtData
{
    [JsonProperty("timeUser")]
    public string TimesUsed { get; set; }
    [JsonProperty("abaNumber")]
    public string AbaNumber { get; set; }
    [JsonProperty("accountNumber")]
    public string AccountNumber { get; set; }
    [JsonProperty("bankName")]
    public string BankName{ get; set; }
    [JsonProperty("beneficiary")]
    public string Beneficiary{ get; set; }
}
