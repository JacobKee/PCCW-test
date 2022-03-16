using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Pccw.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class getWeatherForecastController : Controller
    {
        [HttpGet("{region}")]
        public async Task<string> Get(string region)
        {
            string externalApi = "https://api.data.gov.sg/v1/environment/24-hour-weather-forecast";

            var client = new RestClient(externalApi);
            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");

            var respond = await client.ExecuteAsync(request);
            var Data = respond.get_Content();

            JObject tmpResult = JObject.Parse(Data);
            var general_forecast = tmpResult["items"].Children()["general"].Select(n => n["forecast"]).FirstOrDefault().ToString();
            JObject data = new JObject  {["general_forecast"] = general_forecast };

            var item = tmpResult["items"].Children()["periods"].Children().ToArray();

            JArray mainObj = new JArray();
            foreach (var parent in item)
            {
                JObject obj1 = new JObject();
                foreach (JProperty child in parent)
                {

                    if (child.Name == "time")
                    {
                        obj1 = new JObject(new JProperty(child.Name, child.Select(n => n).FirstOrDefault()));
                    }
                    else
                    {
                        JObject obj2 = new JObject(new JProperty(child.Name, child.Select(n => n[region]).FirstOrDefault()));
                        obj1.Merge(obj2, new JsonMergeSettings
                        {
                            // union array values together to avoid duplicates
                            MergeArrayHandling = MergeArrayHandling.Union
                        });
                    }
                }
                mainObj.Add(obj1);
                System.Diagnostics.Debug.WriteLine(mainObj.ToString());
            }

            JObject obj = new JObject { ["period"] = mainObj };

            data.Merge(obj, new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });
            return JsonConvert.SerializeObject(data); ;
        }
    }
}