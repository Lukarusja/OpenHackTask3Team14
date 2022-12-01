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
    public static class GetRating
    {
        [FunctionName("GetRating")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ratings/{id}")] HttpRequest req,
            [CosmosDB("task3databasenew", "task3containernew", ConnectionStringSetting = "ConnectionStringTask3", SqlQuery = "Select * from ratings r where r.id = {id}")] IEnumerable<RatingModel> rating,
            ILogger log)
        {
            log.LogInformation("Getting Rating");
            if (rating == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(rating);
            }
        }
    }

    public class RatingModel
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string productId { get; set; }
        public DateTime timestamp { get; set; }
        public string timeStamp { get; internal set; }
        public string locationName { get; set; }
        public int rating { get; set; }
        public string userNotes { get; set; }
    }
}
