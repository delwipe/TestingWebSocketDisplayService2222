using Microsoft.AspNetCore.SignalR;
using TestingWebSocketDisplayService2222.Models;
using TestingWebSocketServiceDisplay2222.Models;

namespace TestingWebSocketServiceDisplay2222.Hubs
{
    public class MyHub : Hub
    {
        public async Task UpdateData(Dictionary<string, Models.Event> data)
        {
            await Clients.All.SendAsync("UpdateData", data);
        }

        //public async Task UpdateDataOffers(Dictionary<string, Offer> data)
        //{
        //    await Clients.All.SendAsync("UpdateDataOffers", data);
        //}

    }
}
