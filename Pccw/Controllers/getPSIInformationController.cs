using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Pccw.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class getPSIInformationController : ControllerBase
    {
        // GET getPSIInformation/region
        [HttpGet("{region}")]
        public async Task<string> Get(string region)
        {
            string externalApi = "https://api.data.gov.sg/v1/environment/psi";

            var client = new RestClient(externalApi);
            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");

            var respond = await client.ExecuteAsync(request);
            var Data = respond.get_Content();

            JObject tmpResult = JObject.Parse(Data);
            JObject name = new JObject { ["region"] = tmpResult["region_metadata"].Where(n => n["name"].Value<string>() == region).Select(n => n["name"]).FirstOrDefault() };
            JObject label_location = new JObject { ["location"] = tmpResult["region_metadata"].Where(n => n["name"].Value<string>() == region).Select(n => n["label_location"]).FirstOrDefault() };
            name.Merge(label_location);

            var children = tmpResult["items"].Children()["readings"].Children();
            foreach (JProperty child in children)
            {
                JObject ObjDelParams = new JObject(new JProperty(child.Name, child.Select(n => n[region]).FirstOrDefault()));
                name.Merge(ObjDelParams, new JsonMergeSettings
                {
                    // union array values together to avoid duplicates
                    MergeArrayHandling = MergeArrayHandling.Union
                });
            }

            return JsonConvert.SerializeObject(name);
        }


    }
}