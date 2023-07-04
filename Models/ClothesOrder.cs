using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BackEnd.Models
{
    
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Product
        {
            public string name { get; set; }
            public int price { get; set; }
            public string size { get; set; }
            public string color { get; set; }
            public string image { get; set; }
            public string quantity { get; set; }

    }

        public class ClothesOrder
        {
            public string id { get; set; }
            public string type { get; set; }
            public List<Product> products { get; set; }
        }

    public class Log
    {

        public string id { get; set; }
        public string type { get; set; }
        public int sequence { get; set; }
        public string action { get; set; }
        public string timestamp { get; set; }
        public string user { get; set; }
        public ClothesOrder order { get; set; }

    }


}
