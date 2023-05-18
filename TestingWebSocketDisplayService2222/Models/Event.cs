using Newtonsoft.Json;
using System.Collections;

namespace TestingWebSocketDisplayService2222.Models
{
    public class Event
    {
        public string sport { get; set; }
        public string event_id { get; set; }
        public int competition_id { get; set; }
        public string competition_name { get; set; }
        public string competition_country { get; set; }
        public string home { get; set; }
        public string away { get; set; }
        public string ir_status { get; set; }
        public string start_time { get; set; }
        public static bool TryParse(object obj, out Event eventObj)
        {
            try
            {
                eventObj = JsonConvert.DeserializeObject<Event>(obj.ToString());
                return true;
            }
            catch
            {
                eventObj = null;
                return false;
            }
        }

     
    }

}
