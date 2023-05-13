using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using TestingWebSocketServiceDisplay2222.Hubs;
using Newtonsoft.Json;
using TestingWebSocketServiceDisplay.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TestingWebSocketServiceDisplay2222.Hubs;
using TestingWebSocketServiceDisplay2222.Models;

namespace TestingWebSocketServiceDisplay.Services
{
    public class MyBackgroundTask : IHostedService
    {
        public readonly ConcurrentDictionary<string, Event> _data;
        private readonly IHubContext<MyHub> _hubContext;
        private readonly string _webSocketUrl;
        public MyBackgroundTask(ConcurrentDictionary<string, Event> data, IHubContext<MyHub> hubContext, string webSocketUrl)
        {
            _data = data;
            _hubContext = hubContext;
            _webSocketUrl = webSocketUrl;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            List<byte> messageBuffer = new List<byte>();
            Task.Run(async () =>
            {
                using (var client = new ClientWebSocket())
                {
                    await client.ConnectAsync(new Uri("wss://api.mollybet.com/v1/stream/?token=0b3d091f40ec64f8417c94603d85e5a5"), cancellationToken);
                    int counterBlocks = 0;
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var buffer = new byte[4 * 1024];
                            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                            messageBuffer.AddRange(buffer.Take(result.Count));
                            if (result.EndOfMessage)
                            {
                                string message2 = Encoding.UTF8.GetString(messageBuffer.ToArray());
                                messageBuffer.Clear();

                                // Do something with the parsed message
                                Console.WriteLine(message2);
                                try
                                {
                                    counterBlocks++;
                                    if (message2.Contains("remove_event"))
                                    {

                                    }
                                    if (message2.Contains("balance") && message2.Contains("open_stake"))
                                    {
                                        var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(message2);
                                    }
                                    else if (message2.Contains("[\"sync\", { \"token\": \"]"))
                                    {
                                        string end = "END";
                                    }
                                    else if (message2.Contains("xrate") && message2.Contains("ccy"))
                                    {
                                        //await SetRates(message2);
                                    }
                                    else if (message2.Contains("offer"))
                                    {
                                        // await SetOffer(message2);
                                    }
                                    else if (message2.Contains("remove_event"))
                                    {
                                        _data.Clear();
                                        // await RemoveEvent(message2);
                                    }
                                    else if (message2.Contains("info"))
                                    {
                                        _data.Clear();
                                    }
                                    else if (!message2.Contains("remove_event") && !message2.Contains("[\r\n\"info\",\r\n{"))
                                    {
                                        if (RootObject.TryParse(message2, out RootObject myObj))
                                        {
                                            RootObject root = JsonConvert.DeserializeObject<RootObject>(message2);
                                            // Access the data
                                            foreach (object[] obj in root.data)
                                            {
                                                string dataType = (string)obj[0];
                                                Event eventObj = new();
                                                switch (dataType)
                                                {
                                                    case "event":
                                                        if (Event.TryParse(obj[1], out eventObj))
                                                        {
                                                            string keyEvent = eventObj.sport + "_" + eventObj.event_id;
                                                            if (!_data.ContainsKey(keyEvent))
                                                            {
                                                                _data.TryAdd(keyEvent, eventObj);


                                                                // Notify all connected clients of the updated data using SignalR

                                                                await _hubContext.Clients.All.SendAsync("UpdateData", _data);
                                                            }
                                                            else
                                                            {
                                                                var containsVal = _data[keyEvent];
                                                                _data.TryUpdate(keyEvent, eventObj, containsVal);
                                                            }
                                                        }
                                                        break;
                                                    case "event_time":
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Greska pri deserijalizaciji.");
                                        }

                                    }
                                    else
                                    {
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Exception in: " + ex.Message);
                                }

                            }
                        }
                        catch (WebSocketException)
                        {
                            // Handle WebSocket exceptions
                        }

                        // Wait for a short interval before reading data again
                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    }
                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
