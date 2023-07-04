using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class LogRecord
    {
        public string id { get; set; }
        public string customerName { get; set; }
        public string billingContact { get; set; }
        public string email { get; set; }
        public string shippingContact { get; set; }
        public string creditCardInfo { get; set; }
        public DateTime orderStarted { get; set; }
        public string notes { get; set; }
        public string shipPayType { get; set; }
        public string shipPayAccNum { get; set; }
        public DateTime deliveryDate { get; set; }
        public string customerBillingAddress { get; set; }
        public string customerShippingAddress { get; set; }
        public string shippingTerms { get; set; }
        public string paymentTerms { get; set; }
        public int customerID { get; set; }
        public string salesOrderNumber { get; set; }
        public int currentSeq { get; set; }
        public int docEntry { get; set; }
        public int itemLine { get; set; }
        public List<object> eventLog { get; set; }
        public string type { get; set; }
        public double total { get; set; }

    }
}
