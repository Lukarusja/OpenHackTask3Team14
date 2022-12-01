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
using System.Linq;

namespace OpenHack
{
    public static class GetRatingsFunctions
    {
        [FunctionName(nameof(GetRatings))]
        public static IActionResult GetRatings(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: @"task3databasenew", collectionName: @"task3containernew", ConnectionStringSetting = @"ConnectionStringTask3")] IEnumerable<JObject> allRatings)
        {
            string userId = null;

            if (req.GetQueryParameterDictionary()?.TryGetValue(@"userId", out userId) == true
                && !string.IsNullOrWhiteSpace(userId))
            {
                var userRatings = allRatings.Where(r => r.Value<string>(@"userId") == userId);

                return !userRatings.Any() ? new NotFoundObjectResult($@"No ratings found for user '{userId}'") : (IActionResult)new OkObjectResult(userRatings);

            }
            else
            {
                return new BadRequestObjectResult(@"userId is required as a query parameter");
            }
        }
    }
}
