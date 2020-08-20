using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace re_platfrom_fapp_download_blob
{
    public static class Function1
    {
        [FunctionName("retail_sales_statement")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var name = req.Headers["Authentication"];
            if (name == "jgkbih4VXqJO1nCN8l4X4m1UqIOWcQj15zbOx3fxYhk=")
            {
                string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=redatalake;AccountKey=AnWrOm+WVdGEP3POg76d5eIoNaW676NCSiBsDdcmYw3R7Bz5+WkGNL63VQx5Zg4acw2qf4aEkzOfysYdkaFtxg==;EndpointSuffix=core.windows.net";

                try
                {
                    var dt = DateTime.Now;
                    var result = string.Empty;
                    if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
                    {
                        
                        CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                        string foldercontainer = "100-msd";
                        CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(foldercontainer);
                        await cloudBlobContainer.CreateIfNotExistsAsync();
                        var blobName = "Evolve/RetailSalesStatement_" + dt.ToString("dd-MM-yyyy") + ".csv";
                        CloudBlockBlob block = cloudBlobContainer.GetBlockBlobReference(blobName);
                        HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
                        Stream blobStream = await block.OpenReadAsync();
                        message.Content = new StreamContent(blobStream);
                        message.Content.Headers.ContentLength = block.Properties.Length;
                        message.StatusCode = HttpStatusCode.OK;
                        message.Content.Headers.ContentType = new MediaTypeHeaderValue(block.Properties.ContentType);
                        message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = $"CopyOf_{block.Name}",
                            Size = block.Properties.Length
                        };
                        return message;
                    }
                    else
                    {
                        
                        HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        message.Content = new StringContent("error occured");
                        return message;
                    }

                }
                catch (Exception e)
                {
                    log.LogInformation(e.Message);

                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(e.Message)
                    };
                }
            }
            else
            {

                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Enter valid Authentication key in header")
                };

            }
        }
    }
}

