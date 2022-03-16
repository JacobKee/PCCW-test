using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Pccw.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class getTopicCountController : ControllerBase
    {
        // GET getTopicCount/topic
        [HttpGet("{topic}")]
        public async Task<int> Get(string topic)
        {
            string externalApi = "https://en.wikipedia.org/w/api.php?action=parse&section=0&prop=text&format=json&page=" + topic;

            var client = new RestClient(externalApi);
            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");

            var respond = await client.ExecuteAsync(request);
            var Data = respond.get_Content();
            JObject tmpResult = JObject.Parse(Data);
            var text = tmpResult["parse"]["text"].ToString();
            var count = Regex.Matches(text, topic).Count;

            return count;
        }


    }
}