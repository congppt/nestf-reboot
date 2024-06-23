using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Util;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NestF.S3ImageHandler;

public class Function
{
    IAmazonS3 S3Client { get; set; }
    
    public Function()
    {
        S3Client = new AmazonS3Client(RegionEndpoint.APSoutheast1);
    }
    
    public Function(IAmazonS3 s3Client)
    {
        this.S3Client = s3Client;
    }

    public async Task FunctionHandler(S3Event evnt, ILambdaContext context)
    {
        var eventRecords = evnt.Records ?? new List<S3Event.S3EventNotificationRecord>();
        foreach (var record in eventRecords)
        {
            var s3Event = record.S3;
            if (s3Event == null)
            {
                continue;
            }

            try
            {
                var response = await S3Client.GetObjectMetadataAsync(s3Event.Bucket.Name, s3Event.Object.Key);
                context.Logger.LogInformation(response.Headers.ContentType);
                context.Logger.LogInformation(s3Event.Object.Key + "hit lambda");
                await SendWebhookEvent(s3Event.Object.Key);
            }
            catch (Exception e)
            {
                context.Logger.LogError(
                    $"Error getting object {s3Event.Object.Key} from bucket {s3Event.Bucket.Name}. Make sure they exist and your bucket is in the same region as this function.");
                context.Logger.LogError(e.Message);
                context.Logger.LogError(e.StackTrace);
                throw;
            }
        }
    }

    public async Task SendWebhookEvent(string imagePath)
    {
        var parameters = imagePath.Split(['/', '_']);
        var uri = Constants.HOST;
        var id = int.Parse(parameters[1]);
        switch (parameters[0])
        {
            case Constants.PRODUCT_IMG_FOLDER:
                uri += $"api/products/{id}";
                break;
        }
        var payload = JsonSerializer.Serialize(new { imagePath });
        var client = new HttpClient();
        client.BaseAddress = new Uri(Constants.HOST);
        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PatchAsync(uri, content);
        response.EnsureSuccessStatusCode();
    }
}

class Constants
{
    public const string PRODUCT_IMG_FOLDER = "product-images";
    public const string HOST = "https://nest-f-f8fbc9gbf2g5dteu.southeastasia-01.azurewebsites.net/";
}