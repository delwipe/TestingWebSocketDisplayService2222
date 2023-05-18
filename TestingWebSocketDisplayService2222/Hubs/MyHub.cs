using Microsoft.AspNetCore.SignalR;
using TestingWebSocketDisplayService2222.Models;

namespace TestingWebSocketDisplayService2222.Hubs
{
    public class MyHub : Hub
    {
        public async Task UpdateData(Dictionary<string, Models.Event> data)
        {
            await Clients.All.SendAsync("UpdateData", data);
        }
        public async Task GetAllData(Dictionary<string, Models.Event> data)
        {
            await Clients.All.SendAsync("GetAllData", data);
        }
        public async Task UpdateDataNEW(string key, Models.Event updatedData)
        {
            // Obrada ažuriranog podatka i slanje klijentima
            await Clients.All.SendAsync("UpdateDataNEW", key, updatedData);
        }



        //public async Task UpdateDataOffers(Dictionary<string, Offer> data)
        //{
        //    await Clients.All.SendAsync("UpdateDataOffers", data);
        //}

    }
}
