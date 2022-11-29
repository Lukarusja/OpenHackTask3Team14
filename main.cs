using System;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace OpenHackTask3
{
   // static readonly HttpClient client = new HttpClient();
    public static class CreateRating
    {
        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req,
            ILogger log)
        {

            string bodyContent = req.Content.ReadAsStringAsync().Result;
            JObject bodyJson = JObject.Parse(bodyContent);
            string bodyProductId = bodyJson.GetValue("productId").ToString();
            string bodyUserId = bodyJson.GetValue("userId").ToString();

            string jsonString = "";
            //bool ifProductFound = false;
           // bool ifUserFound = false;
            using HttpClient client = new HttpClient();
            var Products = await client.GetAsync("https://serverlessohapi.azurewebsites.net/api/GetProducts");


            if (Products != null)
            {
                jsonString = Products.Content.ReadAsStringAsync().Result;
            }
    
            JArray json = JArray.Parse(jsonString);

            /*
            foreach (JObject item in json)
            {
                if (item.GetValue("productId").ToString() == bodyProductId)
                    ifProductFound = true;

                if (item.GetValue("userId").ToString() == bodyUserId)
                    ifUserFound = true;
            }
            */

            if (!json.Contains(bodyUserId) || !json.Contains(bodyProductId))
            {
                var message = string.Format("Invalid ProductID or UserID");
                //return req.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, message);
            }

            if (!req.Method.Equals("POST"))
            {
                var message = string.Format("Bad method, use POST instead");
                //return req.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, message);
            }
                log.LogInformation("C# HTTP trigger function processed a request.");

            HttpContent content = req.Content;
            string jsonContent = content.ReadAsStringAsync().Result;

            string responseMessage = string.IsNullOrEmpty("a")
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, shalom. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
