using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AirInfoApi.Controllers
{
    public class ProductController : ApiController
    {
        [Authorize]
        [Route ("api/products", Name = "getProducts")]
        public IEnumerable<string> getProducts()
        {
            return new string[] {"Product 1", "Product 2", "Product 3"};
        }
    }


}
