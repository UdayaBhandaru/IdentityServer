using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ApiCLient2.Controllers
{
    [Route("[controller]")]
    public class ValuesController : Controller
    {
        [Authorize]
        [Route("Welcome")]
        public string Welcome()
        {
            var role = User.Claims?.FirstOrDefault(x => x.Type == "role")?.Value;
            var name = User.Claims?.FirstOrDefault(x => x.Type == "name")?.Value;
            return $"Welcome Home{name}.and your role is {role}.";
        }
        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
