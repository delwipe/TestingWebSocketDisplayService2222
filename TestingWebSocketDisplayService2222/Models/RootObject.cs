using Newtonsoft.Json;

namespace TestingWebSocketServiceDisplay.Models
{
    public class RootObject
    {
        public double ts { get; set; }
        public List<object[]> data { get; set; }
        public static bool TryParse(string json, out RootObject obj)
        {
            try
            {
                obj = JsonConvert.DeserializeObject<RootObject>(json);
                return true;
            }
            catch
            {
                obj = null;
                return false;
            }
        }
    }
}
