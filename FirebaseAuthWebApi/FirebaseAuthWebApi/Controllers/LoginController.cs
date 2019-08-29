using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Http;

namespace FirebaseAuthWebApi.Controllers
{
    public class LoginController : ApiController
    {
        // POST: api/Login
        [Authorize]
        [HttpPost]
        public void Post(object value)
        {
            ClaimsIdentity userIdentity = User.Identity as ClaimsIdentity;
            string userEmail = value.ToString();
        }

        // GET: api/Login
        [AllowAnonymous]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
