using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using BackEnd.Models;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Microsoft.IdentityModel.Protocols;

namespace FunctionHandleCosmosDB
{
    public static class Function1
    {
        static private string CosmosEndPointUri = Environment.GetEnvironmentVariable("Cosmos:EndPointUri");
        static private string CosmosPrimaryKey = Environment.GetEnvironmentVariable("Cosmos:PrimaryKey");

        // The Cosmos client instance
        static CosmosClient SonacosmosClient;

        // The database we will create
        static Database Sonadatabase;

        // The container we will update.
        static Container Sonacontainer;

        // The name of the database and container we will update
        static string SonadatabaseId = "SONA";
        static string SonacontainerId = "DataContainer";


        [FunctionName("RetrieveData")]
        public static async Task<IActionResult> RetrieveData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "RetrieveData")] HttpRequest req,
            ILogger log)
        {
            string responseMessage = string.Empty;

            Response INResponse = new Response();

            try
            {

                if (SonacosmosClient == null)
                    SonacosmosClient = new CosmosClient(CosmosEndPointUri, CosmosPrimaryKey, new CosmosClientOptions() { ApplicationName = "RetrieveData" });
                if (Sonadatabase == null)
                    Sonadatabase = SonacosmosClient.GetDatabase(SonadatabaseId);
                if (Sonacontainer == null)
                    Sonacontainer = Sonadatabase.GetContainer(SonacontainerId);

                try
                {
                    string sqlQueryText = (string.Format("SELECT * FROM c WHERE c.type = 'ExternalOrder'"));

                    QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
                    FeedIterator<LogRecord> queryResultSetIterator = Sonacontainer.GetItemQueryIterator<LogRecord>(queryDefinition);

                    List<LogRecord> items = new List<LogRecord>();

                    while (queryResultSetIterator.HasMoreResults)
                    {
                        FeedResponse<LogRecord> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                        foreach (LogRecord PItem in currentResultSet)
                        {
                            items.Add(PItem);
                        }
                    }

                    INResponse.statusCode = "200";
                    INResponse.response = JsonConvert.SerializeObject(items);

                    return new OkObjectResult(INResponse);
                }
                catch (CosmosException Ex)
                {
                    responseMessage = (string.Format("Exception calling Cosmos DB, Details:{0}", Ex.Message));
                    return new OkObjectResult(responseMessage);
                }
            }
            catch (Exception Ex)
            {
                responseMessage = (string.Format("Exception calling Cosmos DB, Details:{0}", Ex.Message));
                return new OkObjectResult(responseMessage);
            }

        }

        [FunctionName("CreateLogRecord")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
        {
            try
            {
                // Parse request body
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var logRecord = JsonConvert.DeserializeObject<LogRecord>(requestBody);

                // Connect to Cosmos DB
                var client = new CosmosClient(CosmosEndPointUri, CosmosPrimaryKey);
                var container = client.GetContainer(SonadatabaseId, SonacontainerId);

                // Create new document in container
                var response = await container.CreateItemAsync(logRecord);

                return new OkObjectResult($"Log record created with id: {response.Resource.id}");
            }
            catch (Exception ex)
            {
                return new OkObjectResult(ex.Message);
            }
        }

        [FunctionName("RetrieveClothesOrderTest")]
        public static async Task<IActionResult> RetrieveClothesOrderTest(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "RetrieveClothesOrderTest")] HttpRequest req,
    ILogger log)
        {
            string clientId = "68cfa875-a681-450b-8135-e5e0ed477b37";
            string redirectUri = "https://functionhandlecosmosdb.azurewebsites.net/.auth/login/aad/callback";



            // Verificar si se proporcionó un código de autorización en los parámetros de consulta
            if (req.Query.ContainsKey("code"))
            {
                string authorizationCode = req.Query["code"];



                // Aquí puedes guardar el código de autorización y utilizarlo para obtener un token de acceso



                // Tu lógica existente para recuperar los pedidos de ropa
                string responseMessage = string.Empty;
                Response INResponse = new Response();



                try
                {
                    if (SonacosmosClient == null)
                        SonacosmosClient = new CosmosClient(CosmosEndPointUri, CosmosPrimaryKey, new CosmosClientOptions() { ApplicationName = "RetrieveClothesOrderTest" });
                    if (Sonadatabase == null)
                        Sonadatabase = SonacosmosClient.GetDatabase(SonadatabaseId);
                    if (Sonacontainer == null)
                        Sonacontainer = Sonadatabase.GetContainer(SonacontainerId);



                    try
                    {
                        string sqlQueryText = (string.Format("SELECT * FROM c WHERE c.type = 'ClothesOrder'"));



                        QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
                        FeedIterator<ClothesOrder> queryResultSetIterator = Sonacontainer.GetItemQueryIterator<ClothesOrder>(queryDefinition);



                        List<ClothesOrder> items = new List<ClothesOrder>();



                        while (queryResultSetIterator.HasMoreResults)
                        {
                            FeedResponse<ClothesOrder> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                            foreach (ClothesOrder Pitem in currentResultSet)
                            {
                                items.Add(Pitem);
                            }
                        }



                        INResponse.statusCode = "200";
                        INResponse.response = JsonConvert.SerializeObject(items);



                        return new OkObjectResult(INResponse);
                    }
                    catch (CosmosException Ex)
                    {
                        responseMessage = (string.Format("Exception calling Cosmos DB, Details:{0}", Ex.Message));
                        return new OkObjectResult(responseMessage);
                    }
                }
                catch (Exception Ex)
                {
                    responseMessage = (string.Format("Exception calling Cosmos DB, Details:{0}", Ex.Message));
                    return new OkObjectResult(responseMessage);
                }
            }
            else
            {
                // Construir la URL de redirección a la página de inicio de sesión de Azure AD
                string authorizationUrl = "https://login.microsoftonline.com/26e6cc6b-2157-4879-a8f3-f2fc5a6a1bfd/oauth2/v2.0/authorize";
                string scope = "api://68cfa875-a681-450b-8135-e5e0ed477b37/Order.View.All";



                string redirectUrl = $"{authorizationUrl}?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&scope={scope}";



                // Redirigir al usuario a la página de inicio de sesión de Azure AD
                return new RedirectResult(redirectUrl);
            }
        }

        //[FunctionName("Demofunctest")]
        //public static async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
        //FunctionContext executionContext)
        //{
        //    ILogger logger = (ILogger)executionContext.GetLogger("HttpFunction");
        //    logger.LogInformation("Http trigger function processed a request.");

        //    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //    dynamic data = JsonConvert.DeserializeObject(requestBody);
        //    string name = data?.name;

        //    string responseMessage = string.IsNullOrEmpty(name)
        //        ? "This HTTP triggered function executed successfully. Pass a name in the request body for a personalized response."
        //        : $"Hello, {name}. This HTTP triggered function executed successfully.";

        //    var response = req.CreateResponse(HttpStatusCode.OK);
        //    response.WriteString(responseMessage);

        //    return response;
        //}


        [FunctionName("RetrieveClothesOrder")]
        public static async Task<IActionResult> RetrieveClothesOrder(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "RetrieveClothesOrder")] HttpRequest req,
            ILogger log)
        {
            string responseMessage = string.Empty;

            Response INResponse = new Response();

            try
            {

                if (SonacosmosClient == null)
                    SonacosmosClient = new CosmosClient(CosmosEndPointUri, CosmosPrimaryKey, new CosmosClientOptions() { ApplicationName = "RetrieveClothesOrder" });
                if (Sonadatabase == null)
                    Sonadatabase = SonacosmosClient.GetDatabase(SonadatabaseId);
                if (Sonacontainer == null)
                    Sonacontainer = Sonadatabase.GetContainer(SonacontainerId);

                try
                {
                    string sqlQueryText = (string.Format("SELECT * FROM c WHERE c.type = 'ClothesOrder'"));

                    QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
                    FeedIterator<ClothesOrder> queryResultSetIterator = Sonacontainer.GetItemQueryIterator<ClothesOrder>(queryDefinition);

                    List<ClothesOrder> items = new List<ClothesOrder>();

                    while (queryResultSetIterator.HasMoreResults)
                    {
                        FeedResponse<ClothesOrder> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                        foreach (ClothesOrder Pitem in currentResultSet)
                        {
                            items.Add(Pitem);
                        }
                    }

                    INResponse.statusCode = "200";
                    INResponse.response = JsonConvert.SerializeObject(items);

                    return new OkObjectResult(INResponse);
                }
                catch (CosmosException Ex)
                {
                    responseMessage = (string.Format("Exception calling Cosmos DB, Details:{0}", Ex.Message));
                    return new OkObjectResult(responseMessage);
                }
            }
            catch (Exception Ex)
            {
                responseMessage = (string.Format("Exception calling Cosmos DB, Details:{0}", Ex.Message));
                return new OkObjectResult(responseMessage);
            }

        }
        [FunctionName("CreateClothesOrder")]
        public static async Task<IActionResult> CreateClothesOrder(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)

        {
            Response INResponse = new Response();
            try
            {
                // Parse request body
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var order = JsonConvert.DeserializeObject<ClothesOrder>(requestBody);

                // Connect to Cosmos DB
                var client = new CosmosClient(CosmosEndPointUri, CosmosPrimaryKey);
                var container = client.GetContainer(SonadatabaseId, SonacontainerId);

                // Create new document in container
                var response = await container.CreateItemAsync(order);

                INResponse.statusCode = "200";
                INResponse.response = JsonConvert.SerializeObject(order);

                return new OkObjectResult(INResponse);
                //return new OkObjectResult($"Log record created with id: {response.Resource.id}");
            }
            catch (Exception ex)
            {
                return new OkObjectResult(ex.Message);
            }
        }

        [FunctionName("EditClothesOrder")]
        public static async Task<IActionResult> EditClothesOrder(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                // Parse request body
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var updatedOrder = JsonConvert.DeserializeObject<ClothesOrder>(requestBody);

                // Connect to Cosmos DB
                var client = new CosmosClient(CosmosEndPointUri, CosmosPrimaryKey);
                var container = client.GetContainer(SonadatabaseId, SonacontainerId);

                // Get the original order from Cosmos DB
                var response = await container.ReadItemAsync<ClothesOrder>(updatedOrder.id, new PartitionKey(updatedOrder.type));
                var originalOrder = response.Resource;

                // Update the original order with the new values
                originalOrder.products = updatedOrder.products;

                // Update the document in the container
                var updateResponse = await container.ReplaceItemAsync(originalOrder, originalOrder.id, new PartitionKey(originalOrder.type));
                return new OkObjectResult(updatedOrder);
                //return new OkObjectResult($"Order {originalOrder.id} updated successfully");
            }
            catch (Exception ex)
            {
                return new OkObjectResult(ex.Message);
            }
        }

        [FunctionName("RemoveClothesOrder")]
        public static async Task<IActionResult> RemoveClothesOrder(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "orders/{id}/{type}")] HttpRequest req, ILogger log, string id, string type)
        {
            try
            {
                // Create Cosmos DB client
                SonacosmosClient = new CosmosClient(CosmosEndPointUri, CosmosPrimaryKey);

                // Get the database
                Sonadatabase = await SonacosmosClient.CreateDatabaseIfNotExistsAsync(SonadatabaseId);

                // Get the container
                Sonacontainer = await Sonadatabase.CreateContainerIfNotExistsAsync(SonacontainerId, "/type");

                // Get the order from Cosmos DB
                ItemResponse<ClothesOrder> response = await Sonacontainer.ReadItemAsync<ClothesOrder>(id, new PartitionKey(type));

                if (response.Resource == null)
                    return new NotFoundObjectResult($"Order {id} not found");

                // Delete the order
                await Sonacontainer.DeleteItemAsync<ClothesOrder>(id, new PartitionKey(type));
                // Supongamos que tienes una variable 'id' con el valor correspondiente
                string json = $"{{\"OrderDeleted\":\"{id}\"}}";
                return new OkObjectResult(json);
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                    return new NotFoundObjectResult($"Order {id} not found");
                else
                    return new OkObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                return new OkObjectResult(ex.Message);
            }
        }

        [FunctionName("CreateLogs")]
        public static async Task<IActionResult> CreateLogs(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)

        {
            Response INResponse = new Response();
            try
            {
                // Parse request body
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var logs = JsonConvert.DeserializeObject<Log>(requestBody);

                // Connect to Cosmos DB
                var client = new CosmosClient(CosmosEndPointUri, CosmosPrimaryKey);
                var container = client.GetContainer(SonadatabaseId, SonacontainerId);

                // Create new document in container
                var response = await container.CreateItemAsync(logs);

                INResponse.statusCode = "200";
                INResponse.response = JsonConvert.SerializeObject(logs);

                return new OkObjectResult(INResponse);
                //return new OkObjectResult($"Log record created with id: {response.Resource.id}");
            }
            catch (Exception ex)
            {
                return new OkObjectResult(ex.Message);
            }
        }


    }

   
}
