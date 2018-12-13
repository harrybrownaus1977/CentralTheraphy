using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AirInfoApi.Models;

namespace AirInfoApi.Controllers
{
    public class UsersController : ApiController
    {

        private User[] users = new User[]
    {
        new User { id = 1, name = "Haleemah Redfern", email = "email1@mail.com", phone = "01111111", role = 1},
        new User { id = 2, name = "Aya Bostock", email = "email2@mail.com", phone = "01111111", role = 1},
        new User { id = 3, name = "Sohail Perez", email = "email3@mail.com", phone = "01111111", role = 1},
        new User { id = 4, name = "Merryn Peck", email = "email4@mail.com", phone = "01111111", role = 2},
        new User { id = 5, name = "Cairon Reynolds", email = "email5@mail.com", phone = "01111111", role = 3}
    };


        // GET: api/Users
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        public IEnumerable<User> Get()
        {
            return users;
        }

        // GET: api/Users/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Users
        public void Post([FromBody]string value)
        {

        }

        // PUT: api/Users/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Users/5
        public void Delete(int id)
        {
        }
    }
}
