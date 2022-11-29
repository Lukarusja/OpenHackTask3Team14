#r "Newtonsoft.Json"



using System.Net;

using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Primitives;

using Newtonsoft.Json;



public static async Task<IActionResult> Run(HttpRequest req, ILogger log)

{

log.LogInformation("C# HTTP trigger function processed a request.");



string productId = req.Query["productId"];



string requestBody = String.Empty;

using (StreamReader streamReader = new StreamReader(req.Body))

{

requestBody = await streamReader.ReadToEndAsync();

}

dynamic data = JsonConvert.DeserializeObject(requestBody);

productId = productId ?? data?.productId;



string responseMessage = string.IsNullOrEmpty(productId)

? "Please pass a product id on the query string or in the request body"

: $"The product name for your product id {productId} is Starfruit Explosion";



if (req.Method == "POST")

{

responseMessage += " and the description is This starfruit ice cream is out of this world";

}

 



return new OkObjectResult(responseMessage);

}

