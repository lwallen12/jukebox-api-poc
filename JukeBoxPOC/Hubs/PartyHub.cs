using Dapper;
using JukeBoxPOC.Interfaces;
using Microsoft.AspNetCore.SignalR;
using MySql.Data.MySqlClient;
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
            string sql = @"
                    INSERT INTO QueuePOC (PartyName, VideoId, Title, Description,
                                             ImageURL, ChannelTitle, Vote) VALUES 
                                         (@PartyName, @VideoId, @Title, @Description,
                                             @ImageURL, @ChannelTitle, @Vote);
                ";

            using (var connection = new MySqlConnection("Server=test1.ce8cn9mhhgds.us-east-1.rds.amazonaws.com;Database=JukeBox;Uid=Wallen;Pwd=MyRDSdb1;Allow User Variables=True;"))
            {
                try
                {
                    connection.Execute(sql, queue);
                } catch (MySqlException ex)
                {
                    //maybe broadcast error to the client who had tried to add this video?  
                }
                
            }

            await Clients.Group(queue.PartyName).SendAsync("BroadCast", queue);
            //await Clients.All.SendAsync("Broadcast", queue);
        }

        public async Task Vote(VotePackage votePackage)
        {

           await JoinParty(votePackage.PartyName);

            string sql;

            if (votePackage.Action == "Up")
            {
                sql = @"UPDATE QueuePOC 
                            SET Vote = (Vote + 1)
                        WHERE PartyName = @PartyName
                          AND VideoId = @VideoId";
            }
            else
            {
                sql = @"UPDATE QueuePOC 
                            SET Vote = (Vote - 1)
                        WHERE PartyName = @PartyName
                          AND VideoId = @VideoId";
            }

            using (var connection = new MySqlConnection("Server=test1.ce8cn9mhhgds.us-east-1.rds.amazonaws.com;Database=JukeBox;Uid=Wallen;Pwd=MyRDSdb1;Allow User Variables=True;"))
            {
                try
                {
                    connection.Execute(sql, votePackage);
                }
                catch (MySqlException ex)
                {
                    //maybe broadcast error to the client who had tried to add this video?  
                }

            }

            //await Clients.Group(queue.PartyName).SendAsync("BroadCast", queue);
            //await Clients.Group(votePackage.PartyName).SendAsync("NewVote", votePackage);
            await SendVote(votePackage);

        }

        public async Task SendVote(VotePackage votePackage)
        {
            //await JoinParty(votePackage.PartyName);

            // await Clients.Group(votePackage.PartyName).SendAsync("NewVote", votePackage);
            await Clients.All.SendAsync("newvote", votePackage);
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
