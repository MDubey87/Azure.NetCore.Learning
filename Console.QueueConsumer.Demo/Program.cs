using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Text;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
var storageConnectionString = configuration["StorageConnectionString"];
var _queueClient = new QueueClient(storageConnectionString, "attendee-emails");
if(await _queueClient.ExistsAsync())
{
    QueueProperties properties = await _queueClient.GetPropertiesAsync();
    for (int i = 0; i < properties.ApproximateMessagesCount; i++)
    {
        string value = await RetrieveNextMessageAsync();
        Console.WriteLine($"Received: {value}");
    }
}
async Task<string> RetrieveNextMessageAsync()
{
    QueueMessage[] retrievedMessage = await _queueClient.ReceiveMessagesAsync(1);
    var data = Convert.FromBase64String(retrievedMessage[0].Body.ToString());
    string theMessage = Encoding.UTF8.GetString(data);

    await _queueClient.DeleteMessageAsync(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt);

    return theMessage;
}