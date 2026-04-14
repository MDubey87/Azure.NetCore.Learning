using Azure.Data.Tables;
using Azure.Storage.Queues;
using Microsoft.Extensions.Azure;
using Mvc.AzureStorage.Demo.Services;

var builder = WebApplication.CreateBuilder(args);
var storageConnectionString = builder.Configuration["StorageConnectionString"];
builder.Services.AddAzureClients(builder =>
{
    builder.AddBlobServiceClient(storageConnectionString);
    builder.AddQueueServiceClient(storageConnectionString)
    .ConfigureOptions(options => {
        options.MessageEncoding = QueueMessageEncoding.Base64;
    });
    builder.AddTableServiceClient(storageConnectionString);
});

builder.Services.AddAzureClients(b => {
    b.AddClient<QueueClient, QueueClientOptions>((_, _, _) =>
    {
        return new QueueClient(storageConnectionString,
                builder.Configuration["AzureStorage:QueueName"],
                new QueueClientOptions
                {
                    MessageEncoding = QueueMessageEncoding.Base64
                });
    });

    b.AddClient<TableClient, TableClientOptions>((_, _, _) =>
    {
        return new TableClient(storageConnectionString,
                builder.Configuration["AzureStorage:TableStorage"]);
    });
});


builder.Services.AddScoped<ITableStorageServcie, TableStorageServcie>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IQueueService, QueueService>();
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
