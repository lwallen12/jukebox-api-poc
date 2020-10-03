using JukeBoxPOC.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JukeBoxPOC.Hubs
{
    public class PartyHub : Hub
    {

        //public async Task AddToQueue()
        //{
        //    //this needs to first try and add to DB

        //    //second to broadcast as websocket event.. so a listener needs to be on the client for when this 
        //    //is called instead of just adding to that static list. Also, this should not broadcast if failed to add
        //    //to db
        //    //this will only fire if db insert is successful


        //}

        public async Task BroadcastFromClient(object obj)
        {
            await Clients.All.SendAsync("Broadcast", "HOWDY");
        }   

        public async Task JoinParty(string partyName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, partyName);
        }

        public async Task AddToQueue(Queue queue)
        {
            //await Clients.Group(queue.PartyName).SendAsync("BroadCast", queue);
            await Clients.All.SendAsync("Broadcast", queue);
        }


    }

    public class VotePackage
    {
        public string PartyName { get; set; }
        public string VideoId { get; set; }
        public string Action { get; set; }
    }

    public class Queue
    {
        public string PartyName { get; set; }
        public string VideoId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public string ChannelTitle { get; set; }
        public int Vote { get; set; }
    }


}
