using Microsoft.AspNetCore.SignalR;

namespace TestingWebSocketServiceDisplay2222.Hubs
{
    public class MyHub : Hub
    {
        public async Task UpdateData(Dictionary<string, Models.Event> data)
        {
            await Clients.All.SendAsync("UpdateData", data);
        }
    }
}
