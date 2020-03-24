using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace sample_cdn_api_dotnetcore.Controllers
{
    [Route("api/Value")]
    [ApiController]
    public class ValueController : ControllerBase
    {
        private ILogger<ValueController> _logger;
        private IConfiguration _config;

        public ValueController(ILogger<ValueController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpPost("SaveRequest")]
        public async Task<IActionResult> SaveRequest(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SaveRequest");

            string request = string.Empty;
            using (var reader = new StreamReader(Request.Body))
            {
                request = await reader.ReadToEndAsync();
            }

            var azureTokenProvider = new AzureServiceTokenProvider();
            string accessToken = await azureTokenProvider.GetAccessTokenAsync($"https://{_config["StorageAccountName"]}.blob.core.windows.net", cancellationToken: cancellationToken);
            TokenCredential tokenCredential = new TokenCredential(accessToken);

            StorageCredentials storageCredentials = new StorageCredentials(tokenCredential);

            CloudBlobClient blobClient = new CloudBlobClient(new StorageUri(new Uri($"https://{_config["StorageAccountName"]}.blob.core.windows.net")), storageCredentials);
            CloudBlobContainer container = blobClient.GetContainerReference("loans");
            CloudBlockBlob blob = container.GetBlockBlobReference($"{Guid.NewGuid().ToString("n")}.json");

            await blob.UploadTextAsync(request, cancellationToken);

            return Ok();
        }

        [HttpGet("ResilentExternalCall")]
        public async Task<string> ResilentExternalCall()
        {
            ValueServices service = new ValueServices();
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                var serviceTask = service.GetValue(cts.Token);

                if (!Task.WaitAll(new Task[] { serviceTask }, 2000))
                {
                    cts.Cancel();
                    _logger.LogWarning("ResilentExternalCall:  Overlimit");
                    return await Task.FromResult("-1");
                }
                else
                {
                    _logger.LogDebug("ResilentExternalCall:  From Live");
                    return await Task.FromResult(serviceTask.Result.ToString());
                }
            }
        }
    }
}