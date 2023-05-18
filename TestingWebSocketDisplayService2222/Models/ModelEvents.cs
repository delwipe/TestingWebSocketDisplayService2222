using System.Collections;
using System.Collections.Concurrent;
using TestingWebSocketDisplayService2222.Models;

namespace TestingWebSocketDisplayService2222.Models
{
    public class ModelEvents : IEnumerable<Event>
    {
        private List<Event> _events; // Primer privatnog polja

        public ConcurrentDictionary<string, Event> concurentDictionary;
        public ModelEvents()
        {
            concurentDictionary = new();
        }

        public IEnumerator<Event> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
