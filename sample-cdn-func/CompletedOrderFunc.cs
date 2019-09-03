using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace sample_cdn_func
{
    public static class CompletedOrderFunc
    {
        /// <summary>
        /// A Message from Service Bus has been written once the completed order was saved to blob storage in JSON format
        ///     CorrelationId = User Id
        ///     MessageId = Order Id
        /// </summary>
        /// <param name="myQueueItem">triggering message</param>
        /// <param name="completedOrder">JSON of the completed order</param>
        /// <param name="rescentOrdersBlob">Computed list of orders for person</param>
        /// <returns></returns>
        [FunctionName("CompletedOrderFunc")]
        public static async Task Run([ServiceBusTrigger("myqueue", Connection = "SBConnStr")]string myQueueItem, 
                                     [Blob("completed-orders/{CorrelationId}-{MessageId}.json", System.IO.FileAccess.Read, Connection = "OrdersStorageConnectionString")] string completedOrder,
                                     [Blob("rescent-orders/{CorrelationId}.json", System.IO.FileAccess.ReadWrite, Connection="PreComputeStorageConnectionString")]CloudBlockBlob rescentOrdersBlob, 
                                     ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
