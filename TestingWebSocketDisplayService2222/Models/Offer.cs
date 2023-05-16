using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestingWebSocketDisplayService2222.Models;

namespace TestingWebSocketServiceDisplay2222.Models
{
    public class Offer
    {
        public string bookie { get; set; }
        public string market { get; set; }
        public string selection { get; set; }
        public string sport { get; set; }
        public string event_id { get; set; }
        public string bet_type { get; set; }
        public bool in_running { get; set; }
        public List<PriceListItem> price_list { get; set; }
        public static bool TryParse(object obj, out Offer offerObj)
        {
            try
            {
                offerObj = JsonConvert.DeserializeObject<Offer>(obj.ToString());
                return true;
            }
            catch
            {
                offerObj = null;
                return false;
            }
        }
    }
}
