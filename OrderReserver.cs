using System.Net;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace OrderItemsReserveService
{
    public class OrderReserver
    {
        private readonly ILogger _logger;
        private const string OrderBlobConnection = "WEBSITE_CONTENTAZUREFILECONNECTIONSTRING";
        private const string OrderBlobContainer = "order-container";

        public OrderReserver(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<OrderReserver>();
        }

        [Function("OrderReserver")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            BlobServiceClient blobServiceClient = new(Environment.GetEnvironmentVariable(OrderBlobConnection));
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(OrderBlobContainer);
            containerClient.CreateIfNotExists();
 
            req.Body.Seek(0, SeekOrigin.Begin);
            containerClient.UploadBlob(Path.ChangeExtension(Guid.NewGuid().ToString(), "json"), req.Body);

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}
