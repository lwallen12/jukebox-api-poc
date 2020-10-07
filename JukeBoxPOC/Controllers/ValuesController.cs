using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using JukeBoxPOC.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MySql.Data.MySqlClient;

namespace JukeBoxPOC.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {

        private readonly IHubContext<PartyHub> _partyHub;

        public ValuesController(IHubContext<PartyHub> partyHub)
        {
            _partyHub = partyHub;
        }

        // GET api/values
        [HttpGet("{user}")]
        public IEnumerable<string> Get(string user)
        {
            IEnumerable<string> parties;

            using (var connection = new MySqlConnection("Server=test1.ce8cn9mhhgds.us-east-1.rds.amazonaws.com;Database=JukeBox;Uid=Wallen;Pwd=MyRDSdb1;Allow User Variables=True;"))
            {
                parties = connection.Query<string>($"SELECT PartyName FROM PartyPOC WHERE UserName = '{user}';").ToList();
            }

            return parties;
        }

        [HttpGet("videos/{party}")]
        public IEnumerable<Queue> GetQueueAsync(string party)
        {
            IEnumerable<Queue> queues;

            using (var connection = new MySqlConnection("Server=test1.ce8cn9mhhgds.us-east-1.rds.amazonaws.com;Database=JukeBox;Uid=Wallen;Pwd=MyRDSdb1;Allow User Variables=True;"))
            {
                queues = connection.Query<Queue>($"SELECT * FROM QueuePOC Where PartyName = '{party}';").ToList();
            }

            return queues;
        }

        [HttpGet]
        public async Task TestCall()
        {
           await _partyHub.Clients.All.SendAsync("Test call work");
        }

        //[HttpPost("Add")]
        //public async Task AddToQueue([FromBody] Queue queue)
        //{
            
        //}

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
