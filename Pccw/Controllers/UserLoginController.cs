using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pccw.Model;

namespace Pccw.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {

        // POST UserLogin
        [HttpPost]
        public ActionResult Post([FromBody] User user)
        {
            string Token = Request.Headers["apikey"];

            if (Token != "PCCW$981" || Token == "")
            {
                return new OkObjectResult(new { status = "failed, apikey is not correct" });
            }
            else
            {
                byte[] hash;
                using (MD5 md5 = MD5.Create())
                {
                    hash = md5.ComputeHash(Encoding.UTF8.GetBytes(user.userid + user.password));
                }

                int total = 0;
                foreach (int i in hash)
                {
                    total += i;
                }

                if (total % 2 == 0)
                {
                    return new OkObjectResult(new { status = "failed" });
                }
                else
                {
                    return new OkObjectResult(new { status = "success" });

                }

            }
        }

    }
}
