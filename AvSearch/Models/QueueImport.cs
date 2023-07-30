using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvSearch.Models
{
    public class QueueImport
    {
        public string source { get; set; }
        public string routingNumber { get; set; }
        public string accountNumber { get; set; }
        public string bankName { get; set; }
        public accountHolder accountHolder { get; set; }
        
    }
}
