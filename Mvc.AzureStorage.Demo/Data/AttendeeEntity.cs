using Azure;
using Azure.Data.Tables;

namespace Mvc.AzureStorage.Demo.Data
{
    public class AttendeeEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string ImageName { get; set; } = string.Empty;
    }
}
