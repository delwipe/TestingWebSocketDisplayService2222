using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net.WebSockets;
using System.Text;
using TestingWebSocketDisplayService2222.Hubs;
using TestingWebSocketDisplayService2222.Models;
using TestingWebSocketServiceDisplay.Models;

namespace TestingWebSocketDisplayService2222.Services
{
    public class MyBackgroundTask : BackgroundService
    {
        public ConcurrentDictionary<string, Event>? _data;
        public ConcurrentDictionary<string, Models.Offer>? offers;
        private readonly ILogger<MyBackgroundTask> _logger;
        private readonly IHubContext<MyHub> _hubContext;
        public MyBackgroundTask(ConcurrentDictionary<string, Event> data, IHubContext<MyHub> hubContext, ILogger<MyBackgroundTask> logger)
        {
            offers = new();
            _data = data;
            _hubContext = hubContext;
            _logger = logger;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service started.");        
        }
        private async Task RemoveOffer(string message2)
        {
            if (RootObject.TryParse(message2, out RootObject myObj))
            {
                RootObject? root = JsonConvert.DeserializeObject<RootObject>(message2);
                foreach (object[] obj in root.data)
                {
                    string dataType = (string)obj[0];
                    Offer offerData = new();
                    switch (dataType)
                    {
                        case "remove_offer":
                            if (Offer.TryParse(obj[1], out offerData))
                            {
                                string keyEvent = $"{offerData.sport}_{offerData.event_id}";
                                if (offers.ContainsKey(keyEvent))
                                {
                                    KeyValuePair<string, Offer> offerDelete = new KeyValuePair<string, Offer>(keyEvent, offerData);
                                    offers.TryRemove(offerDelete);
                                }
                                await _hubContext.Clients.All.SendAsync("UpdateDataOffers", offers);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private async Task SetOffer(string message2, ClientWebSocket client)
        {
            bool containsWhatNeed = false;
            List<string> sports = new();
            List<string> eventIds = new();
            if (RootObject.TryParse(message2, out RootObject myObj))
            {
                RootObject? root = JsonConvert.DeserializeObject<RootObject>(message2);
                foreach (object[] obj in root.data)
                {
                    string dataType = (string)obj[0];
                    Offer offerData = new();
                    switch (dataType)
                    {
                        case "offer":
                            if (Offer.TryParse(obj[1], out offerData))
                            {
                                if (!sports.Contains(offerData.sport))
                                {
                                    sports.Add(offerData.sport);
                                }
                                if (!eventIds.Contains(offerData.event_id))
                                {
                                    eventIds.Add(offerData.event_id);
                                }
                                string keyEvent = offerData.sport + "_" + offerData.event_id + "_" + offerData.selection;
                                if (offerData.bet_type == "for,h" || offerData.bet_type == "for,a" || offerData.bet_type == "for,d" || offerData.bet_type == "for,score,both,yes")
                                {
                                    containsWhatNeed = true;
                                }
                                if (!offers.ContainsKey(keyEvent))
                                {
                                    offers.TryAdd(keyEvent, offerData);
                                }
                                else
                                {
                                    offers.TryUpdate(keyEvent, offerData, offers[keyEvent]);
                                }
                                // await _hubContext.Clients.All.SendAsync("UpdateDataOffers", offers);
                            }
                            break;
                        case "response":
                            break;
                        default:
                            break;
                    }
                }
            }
            if (!containsWhatNeed)
            {
                foreach (var sport in sports)
                {
                    foreach (var id in eventIds)
                    {
                       // await RegisterOffer(client,sport, id, false);
                    }
                }
            }
        }
        //true or false for register or unregister
        private async Task RegisterOffer(ClientWebSocket client, string sport, string eventId, bool registerOrUnregister)
        {
            string register = "register_event";
            string unregister = "un" + register;
            string messageToSend = "";
            if (registerOrUnregister)
            {
                messageToSend = register;
            }
            else
            {
                messageToSend = unregister;
            }
            string messageForWS = $"[\"{messageToSend}\", \"{sport}\", \"{eventId}\"]";
            byte[] messageBytes = Encoding.UTF8.GetBytes(messageForWS);
            await client.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        private async Task RemoveEvent(string message2)
        {
            if (RootObject.TryParse(message2, out RootObject myObj))
            {
                RootObject? root = JsonConvert.DeserializeObject<RootObject>(message2);
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
                                string keyEvent = $"{eventObj.sport}_{eventObj.event_id}";
                                if (_data.ContainsKey(keyEvent))
                                {
                                    var result = Task.Run(() => RemoveAsync(keyEvent, eventObj));
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
                _logger.LogWarning("Deserialization error. (MyBackgroundTask)");
            }
        }
        private async Task RemoveAsync(string key, Event eventObj)
        {
            _data.Remove(key, out eventObj);
            // await _hubContext.Clients.All.SendAsync("UpdateData", _data);
            await _hubContext.Clients.All.SendAsync("UpdateDataNEW", key, null);
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service stopped.");
            return Task.CompletedTask;
        }
        private void CheckDate(string utcTimeString, out string dateTimeParsed, out bool isDatePassed)
        {
            dateTimeParsed = "";
            isDatePassed = false; 
            if (DateTime.TryParseExact(utcTimeString, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime parsedDateTime))
            {
                // Convert UTC time to local time
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(parsedDateTime, TimeZoneInfo.Local);
                // Format local time as a string
                string localTimeString = localTime.ToString("dd.MM.yyyy HH:mm:ss");
                dateTimeParsed = localTimeString.ToString();
                if (parsedDateTime < DateTime.Today)
                {
                    isDatePassed = true;
                }
            }
        }
        private async Task CheckEventObject(RootObject root, ClientWebSocket client)
        {
            foreach (object[] obj in root.data)
            {
                string dataType = (string)obj[0];
                Event eventObj = new();
                string dateTimeParsed = "";
                switch (dataType)
                {
                    case "event":
                        if (Event.TryParse(obj[1], out eventObj))
                        {
                            string keyEvent = $"{eventObj.sport}_{eventObj.event_id}";
                            if (eventObj.sport != "cricket")
                            {
                                string utcTimeString = eventObj.start_time;
                                bool isDatePassed = false;
                                CheckDate(utcTimeString, out dateTimeParsed, out isDatePassed);
                                if (!isDatePassed || eventObj.start_time == null)
                                {
                                    if (!_data.ContainsKey(keyEvent))
                                    {
                                        if (dateTimeParsed != "")
                                        {
                                            eventObj.start_time = dateTimeParsed;
                                        }
                                        _data.TryAdd(keyEvent, eventObj);
                                       // await RegisterOffer(client, eventObj.sport, eventObj.event_id, true);
                                        //string messageForWS = $"[\"register_event\", \"{eventObj.sport}\", \"{eventObj.event_id}\"]";
                                        //byte[] messageBytes = Encoding.UTF8.GetBytes(messageForWS);
                                        //await client.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                                        // Notify all connected clie
                                        // nts of the updated data using SignalR
                                        // await _hubContext.Clients.All.SendAsync("UpdateData", _data);
                                        await _hubContext.Clients.All.SendAsync("UpdateDataNEW", keyEvent, eventObj);
                                    }
                                    else
                                    {
                                        var containsVal = _data[keyEvent];
                                        _data.TryUpdate(keyEvent, eventObj, containsVal);
                                        //await _hubContext.Clients.All.SendAsync("UpdateData", _data);
                                        await _hubContext.Clients.All.SendAsync("UpdateDataNEW", keyEvent, eventObj);
                                    }
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
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<byte> messageBuffer = new List<byte>();
            try
            {
                using (var client = new ClientWebSocket())
                {
                    await client.ConnectAsync(new Uri("wss://api.mollybet.com/v1/stream/?token=0b3d091f40ec64f8417c94603d85e5a5"), stoppingToken);
                    int counterBlocks = 0;
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogInformation("Worker running at:{time}", DateTime.Now);
                        try
                        {
                            var buffer = new byte[4 * 1024];
                            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);
                            messageBuffer.AddRange(buffer.Take(result.Count));
                            if (result.EndOfMessage)
                            {
                                string message2 = Encoding.UTF8.GetString(messageBuffer.ToArray());
                                messageBuffer.Clear();
                                Console.WriteLine(message2);
                                try
                                {
                                    counterBlocks++;
                                    if (message2.Contains("balance") && message2.Contains("open_stake"))
                                    {
                                        var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(message2);
                                    }
                                    else if (!message2.Contains("remove_event") && message2.Contains("offer"))
                                    {
                                        await SetOffer(message2, client);

                                    }
                                    else if (message2.Contains("[\"sync\", { \"token\": \"]"))
                                    {
                                        string end = "END";
                                    }
                                    else if (message2.Contains("xrate") && message2.Contains("ccy"))
                                    {
                                        //await SetRates(message2);
                                    }
                                    else if (message2.Contains("event_exchange_dark_liquidity"))
                                    {
                                    }
                                    else if (message2.Contains("remove_offer"))
                                    {
                                        await RemoveOffer(message2);
                                    }
                                    else if (message2.Contains("offer"))
                                    {
                                        await SetOffer(message2, client);
                                    }
                                    else if (message2.Contains("remove_event"))
                                    {
                                        await RemoveEvent(message2);
                                    }
                                    else if (message2.Contains("info"))
                                    {
                                    }
                                    else if (!message2.Contains("remove_event") && !message2.Contains("[\r\n\"info\",\r\n{"))
                                    {
                                        if (RootObject.TryParse(message2, out RootObject myObj))
                                        {
                                            RootObject? root = JsonConvert.DeserializeObject<RootObject>(message2);
                                            // Access the data
                                            await CheckEventObject(root, client);                                        
                                        }
                                        else
                                        {
                                            _logger.LogWarning("Deserialization error. (MyBackgroundTask)");
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogInformation("undefined information");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Exception in: " + ex.Message);
                                }
                            }
                        }
                        catch (WebSocketException wse)
                        {
                            throw new Exception("Exception in StartAsync in MyBackgroundTask: " + wse.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ExecuteAsync: " + ex.Message);
            }
        }


    }
}
