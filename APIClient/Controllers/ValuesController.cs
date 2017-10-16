using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace APIClient.Controllers
{
    [Route("[controller]")]
    
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]

        [Authorize(Roles = "Administrator")]
        [Route("Admin")]
        public string Admin()
        {
            return "You are the Admin Welcome Home.";
        }

        [Authorize(Roles = "Contributor,Administrator")]
        [Route("Contribute")]
        public string Contribute()
        {
            return "You are the Contributor. Welcome Home.";
        }

        [Authorize(Roles = "User")]
        [Route("Reader")]
        public string Reader()
        {
            return "You are the User. Welcome Home.";
        }


        [Authorize]
        [Route("Welcome")]
        public string Welcome()
        {
            var role = User.Claims?.FirstOrDefault(x => x.Type == "role")?.Value;
            var name = User.Claims?.FirstOrDefault(x => x.Type == "name")?.Value;
            return $"Welcome Home{name}.and your role is {role}.";
        }
        
    }
}
