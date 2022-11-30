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
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure.Cosmos.Table;
using System.Xml.Linq;
using Microsoft.Azure.Documents;
using Microsoft.AspNetCore.Http;

namespace OpenHack
{
    public static class CreateRating
    {
      
        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req, [CosmosDB(
        databaseName: "task3databasenew",
        collectionName: "task3containernew",
        ConnectionStringSetting = "ConnectionStringTask3")]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {

            // check if the Method is POST, if not, throw 405 error

            if (!req.Method.Equals("POST"))
            {

                return new BadRequestObjectResult("Bad method, use POST"); //400
            }

            var bodyContent = req.Content.ReadAsStringAsync().Result;
            JObject bodyJson = JObject.Parse(bodyContent);
            
            // store productId and userId from request
            string bodyProductId = bodyJson.GetValue("productId").ToString();
            string bodyUserId = bodyJson.GetValue("userId").ToString();
            

            string jsonString = "";
            bool ifProductFound = false;
            bool ifUserFound = false;

            // obtain all product IDs
            using HttpClient client = new HttpClient();
            var Products = await client.GetAsync("https://serverlessohapi.azurewebsites.net/api/GetProducts");


            if (Products != null)
            {
                jsonString = Products.Content.ReadAsStringAsync().Result;
            }

            // this array contains all product IDs
            JArray jsonProducts = JArray.Parse(jsonString);



            // obtain all user IDs
            var Users = await client.GetAsync("https://serverlessohapi.azurewebsites.net/api/GetUsers");


            if (Users != null)
            {
                jsonString = Users.Content.ReadAsStringAsync().Result;
            }


            // this array contains all user IDs
            JArray jsonUsers = JArray.Parse(jsonString);



            // check if product Id from body exists in the external API
            foreach (JObject item in jsonProducts)
            {
                if (item.GetValue("productId").ToString().Equals(bodyProductId))
                    ifProductFound = true;

            }

            // check if user Id from body exists in the external API
            foreach (JObject item in jsonUsers)
            {                
                if (item.GetValue("userId").ToString().Equals(bodyUserId))
                    ifUserFound = true;
            }

            if (!ifUserFound || !ifProductFound)
            {
                  // throw 404
                return new NotFoundResult();
            }
            else
            {

                try {
                    await documentsOut.AddAsync(new
                    {

                        userId = bodyJson.GetValue("userId").ToString(),
                        productId = bodyJson.GetValue("productId").ToString(),
                        locationName = bodyJson.GetValue("locationName").ToString(),
                        rating = bodyJson.GetValue("rating").ToString(),
                        userNotes = bodyJson.GetValue("userNotes").ToString()


                    }); 
                    }
                catch {
                     // throw 400
                    return new BadRequestObjectResult("Duplicate ProductID and UserID"); // 400

                };
            }





               // log.LogInformation("C# HTTP trigger function processed a request.");

            HttpContent content = req.Content;
            string jsonContent = content.ReadAsStringAsync().Result;

            string responseMessage = string.IsNullOrEmpty("a")
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, shalom. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
