using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvSearch
{
    public class GlobalObjects
    {
        public static object AvInquiryJson;
        internal static object OrderId;
        public static object avInquiryJson;
        public static object dataOrderId;
        public static object orderId;
        public static string entityName;

        public static string AccessToken { get; internal set; }
        public static string ResponseObject { get; internal set; }
        public static string ServiceId { get; internal set; }
        public static string RoutingNumber{ get; internal set; }
        public static string AccountNumber { get; internal set; }
        public static string AvAccountId { get; internal set; }
        public static string AccountHolderName { get; internal set; }
        public static string UserName { get; internal set; }
        public static string Password { get; internal set; }
        public static JObject JsonObject { get; set; }
        public static string QueryObject { get; set; }
        public static string Transaction { get; set; }
    }
}
