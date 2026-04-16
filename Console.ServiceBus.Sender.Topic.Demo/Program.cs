using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
const string topicName = "";
const int maxNumberodMessages = 5;
ServiceBusClient serviceBusClient = new ServiceBusClient(configuration["ServiceBusConnectionString"],
new ServiceBusClientOptions
{
    // optionally force websocket if your network blocks 5671/5672
    TransportType = ServiceBusTransportType.AmqpWebSockets
});
ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(topicName);
using ServiceBusMessageBatch messageBatch = await serviceBusSender.CreateMessageBatchAsync();
for (int i = 0; i < maxNumberodMessages; i++)
{
    if (!messageBatch.TryAddMessage(new ServiceBusMessage($"This is message - {i}")))
    {
        Console.WriteLine($"Message - {i} is not added to the batch.");
    }
}
try
{
    await serviceBusSender.SendMessagesAsync(messageBatch);
    Console.WriteLine($"A batch of {maxNumberodMessages} messages has been published to the queue.");
}
catch (Exception ex)
{
    Console.WriteLine($"Exception occured while sending messages: {ex.Message}");
}
finally
{
    await serviceBusSender.DisposeAsync();
    await serviceBusClient.DisposeAsync();
}