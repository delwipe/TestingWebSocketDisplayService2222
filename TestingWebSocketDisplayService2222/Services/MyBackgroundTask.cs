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
using System;
using TestingWebSocketDisplayService2222.Models;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace TestingWebSocketServiceDisplay2222.Services
{
    public class MyBackgroundTask : IHostedService
    {
        public readonly ConcurrentDictionary<string, Event> _data;
        public readonly ConcurrentDictionary<string, Models.Offer> offers;

        private readonly IHubContext<MyHub> _hubContext;
        public MyBackgroundTask(ConcurrentDictionary<string, Event> data, IHubContext<MyHub> hubContext)
        {
            offers = new();
            _data = data;
            _hubContext = hubContext;
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
                                        await SetOffer(message2);
                                    }
                                    else if (message2.Contains("remove_event"))
                                    {
                                        //_data.Clear();
                                        await RemoveEvent(message2);
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
                                                            if (eventObj.sport != "cricket")
                                                            {
                                                                if (!_data.ContainsKey(keyEvent))
                                                                {
                                                                    _data.TryAdd(keyEvent, eventObj);
                                                                    string messageForWS = "[\"register_event\", " + "\"" + eventObj.sport + "\", " + "\"" + eventObj.event_id + "\"]";
                                                                    byte[] messageBytes = Encoding.UTF8.GetBytes(messageForWS);
                                                                    await client.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);

                                                                    // Notify all connected clients of the updated data using SignalR

                                                                    //   await _hubContext.Clients.All.SendAsync("UpdateData", eventObj);

                                                                    await _hubContext.Clients.All.SendAsync("UpdateData", _data);

                                                                }
                                                                else
                                                                {
                                                                    var containsVal = _data[keyEvent];
                                                                    //eventObj.sport = "UPDATED " + eventObj.sport;
                                                                    _data.TryUpdate(keyEvent, eventObj, containsVal);
                                                                }
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
                        await Task.Delay(TimeSpan.FromSeconds(0.5), cancellationToken);
                    }
                }
            }, cancellationToken);

            return Task.CompletedTask;
        }
        private async Task SetOffer(string message2)
        {
            if (RootObject.TryParse(message2, out RootObject myObj))
            {
                RootObject root = JsonConvert.DeserializeObject<RootObject>(message2);
                foreach (object[] obj in root.data)
                {
                    string dataType = (string)obj[0];
                    Offer offer = new();
                    switch (dataType)
                    {
                        case "offer":
                            if (Offer.TryParse(obj[1], out offer))
                            {
                                string keyEvent = offer.sport + "_" + offer.event_id;
                                if (!offers.ContainsKey(keyEvent))
                                {
                                    offers.TryAdd(keyEvent, offer);
                                }
                                else
                                {
                                    offers.TryUpdate(keyEvent, offer, offers[keyEvent]);
                                }
                               // await _hubContext.Clients.All.SendAsync("UpdateDataOffers", offers);

                            }
                            break;
                        default:
                            break;
                    }
                }
            }

        }

        private async Task RemoveEvent(string message2)
        {
            if (RootObject.TryParse(message2, out RootObject myObj))
            {
                // Uspesno deserijalizovan objekat
                Console.WriteLine(myObj.ts + " " + myObj.data);
                RootObject root = JsonConvert.DeserializeObject<RootObject>(message2);
                // Access the data
                foreach (object[] obj in root.data)
                {
                    string dataType = (string)obj[0];
                    Event eventObj = new();

                    switch (dataType)
                    {
                        case "remove_event":
                            if (Event.TryParse(obj[1], out eventObj))
                            {
                                string keyEvent = eventObj.sport + "_" + eventObj.event_id;

                                if (_data.ContainsKey(keyEvent))
                                {
                                    var th = Task.Run(() => RemoveAsync(keyEvent, eventObj));
                                }
                            }
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
        private async Task RemoveAsync(string key, Event eventObj)
        {
            KeyValuePair<string, Event> eventKeyValuePair = new(key, eventObj);
                _data.TryRemove(eventKeyValuePair);
                await _hubContext.Clients.All.SendAsync("UpdateData", _data);

            

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
