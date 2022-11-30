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
using CloudStorageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount;
using CloudTableClient = Microsoft.Azure.Cosmos.Table.CloudTableClient;
using CloudTable = Microsoft.Azure.Cosmos.Table.CloudTable;

namespace OpenHack
{
    public static class Function1
    {
        // My experiments with Table Storage connection
        /*
        static string storageconn = "DefaultEndpointsProtocol=https;AccountName=task3wlad909d;AccountKey=zR46RH7WIRDLt30Uuo7z3IlAW3BVuc2v4z2A4+Gs1FpOOv0FNlxYcEBfQx1RBkBGViWlOTSxxQgW+AStzhl3Zg==;EndpointSuffix=core.windows.net"

        static string table1 = "Ratings";

        static void Main(string[] args)
        {
            CloudStorageAccount storageAcc = CloudStorageAccount.Parse(storageconn);
            CloudTableClient tblclient = storageAcc.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tblclient.GetTableReference(table1);

            Console.ReadKey();
        }
        */

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req,
            ILogger log)
        {

            // check if the Method is POST, if not, throw 405 error

            if (!req.Method.Equals("POST"))
            {
                var message = string.Format("Bad method, use POST instead");
                Console.WriteLine(message);

                // how do I fix it?
                //return req.CreateResponse(HttpStatusCode.MethodNotAllowed, message);
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
                var message = string.Format("Invalid ProductID or UserID");
                Console.WriteLine(message);

                // throw 404
                //return req.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, message);
            }
            else
            {
                // delete this else after we're finished
                Console.WriteLine("good user and prodcu t id");
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
