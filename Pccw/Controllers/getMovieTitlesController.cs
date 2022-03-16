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
    public class getMovieTitlesController : Controller
    {
        // GET getMovieTitles/title
        [HttpGet("{title}")]
        public async Task<string> Get(string title)
        {
            string externalApi = "https://jsonmock.hackerrank.com/api/movies/search/?Title=" + title;

            var client = new RestClient(externalApi);
            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");

            var respond = await client.ExecuteAsync(request);
            var Data = respond.get_Content();
            JObject tmpResult = JObject.Parse(Data);
            var total_pages = tmpResult["total_pages"].ToString();
            var data = tmpResult["data"].ToString();

            for (int i = 2; i <= Int32.Parse(total_pages); i++)
            {
                externalApi = "https://jsonmock.hackerrank.com/api/movies/search/?Title=" + title + "&page=" + i;

                client = new RestClient(externalApi);
                request = new RestRequest();
                request.Method = Method.Get;
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");

                respond = await client.ExecuteAsync(request);
                Data = respond.get_Content();
                tmpResult = JObject.Parse(Data);
                data += tmpResult["data"].ToString();
            }

            return data;
        }
    }
}