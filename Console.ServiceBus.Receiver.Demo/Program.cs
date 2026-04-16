
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
const string queueName = "";

ServiceBusProcessor serviceBusProcessor = default!;
async Task MessageHandler(ProcessMessageEventArgs processMessageEventArgs)
{
    string body = processMessageEventArgs.Message.Body.ToString();
    Console.WriteLine($"Received: {body}");
    await processMessageEventArgs.CompleteMessageAsync(processMessageEventArgs.Message);
}

Task MessageErrorHandler(ProcessErrorEventArgs processErrorEventArgs)
{
    Console.WriteLine($"Message handler encountered an exception {processErrorEventArgs.Exception.ToString()}.");
    return Task.CompletedTask;
}
ServiceBusClient serviceBusClient = new ServiceBusClient(configuration["ServiceBusConnectionString"]);
serviceBusProcessor = serviceBusClient.CreateProcessor(queueName, new ServiceBusProcessorOptions());
try
{
    serviceBusProcessor.ProcessMessageAsync += MessageHandler;
    serviceBusProcessor.ProcessErrorAsync += MessageErrorHandler;
    await serviceBusProcessor.StartProcessingAsync();
    Console.WriteLine("Wait for a minute and then press any key to end the processing");
    Console.ReadKey();
    Console.WriteLine("Stopping the receiver...");
    await serviceBusProcessor.StopProcessingAsync();
    Console.WriteLine("Stopped receiving messages");
}
catch (Exception ex)
{
    Console.WriteLine($"Exception occured while sending messages: {ex.Message}");
}
finally
{
    await serviceBusProcessor.DisposeAsync();
    await serviceBusClient.DisposeAsync();
}