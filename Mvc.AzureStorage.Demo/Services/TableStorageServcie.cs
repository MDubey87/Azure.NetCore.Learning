using Azure;
using Azure.Data.Tables;
using Mvc.AzureStorage.Demo.Data;

namespace Mvc.AzureStorage.Demo.Services
{
    public class TableStorageServcie(IConfiguration configuration, TableServiceClient tableServiceClient, TableClient tableClient) : ITableStorageServcie
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly TableServiceClient _tableServiceClient = tableServiceClient;
        private readonly TableClient _tableClient = tableClient;
        private const string TableName = "Attendees";
        public async Task<AttendeeEntity> GetAttendeeAsync(string industry, string id)           
        {
            var client = _tableServiceClient.GetTableClient(TableName);
            return await client.GetEntityAsync<AttendeeEntity>(industry, id);
        }
        public async Task<List<AttendeeEntity>> GetAttendeesAsync()
        {
            //var client = _tableServiceClient.GetTableClient(TableName);
            Pageable<AttendeeEntity> attendees = _tableClient.Query<AttendeeEntity>();
            return attendees.ToList();
        }
        public async Task UpsertAttendeeAsync(AttendeeEntity attendee)
        {
            //var client = _tableServiceClient.GetTableClient(TableName);
            await _tableClient.UpsertEntityAsync(attendee);
        }

        public async Task DeleteAttendeeAsync(string industry, string id)
        {
            //var client = _tableServiceClient.GetTableClient(TableName);
            await _tableClient.DeleteEntityAsync(industry, id);
        }

        //private async Task<TableClient> GetTableClientAsync()
        //{
        //    var connectionString = _configuration["StorageConnectionString"];
        //    var serviceClient = new TableServiceClient(connectionString);
        //    var tableClient = serviceClient.GetTableClient(TableName);
        //    await tableClient.CreateIfNotExistsAsync();
        //    return tableClient;
        //}
    }
}
