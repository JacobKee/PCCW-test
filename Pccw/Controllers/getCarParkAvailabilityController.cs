using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Pccw.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class getCarParkAvailabilityController : Controller
    {
        [HttpGet]
        public async Task<string> Get()
        {
            string externalApi = "https://api.data.gov.sg/v1/transport/carpark-availability?date_time=" + DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");

            var client = new RestClient(externalApi);
            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");

            var respond = await client.ExecuteAsync(request);
            var Data = respond.get_Content();

            var result = JObject.Parse(Data);
            return result.ToString();
        }
    }
}