using Microsoft.Azure.Cosmos;

namespace Blazor.CosmosDB.Demo.Data
{
    public class EngineerService : IEngineerService
    {
        private readonly string CosmosDbAccountUri = "YOUR_COSMOS_DB_ACOUNTURI";
        private readonly string CosmosDbAccountPrimaryKey = "YOUR_COSMOS_DB_PRIMARY_KEY";
        private readonly string CosmosDbDatabaseName = "YOUR_COSMOS_DB_NAME";
        private readonly string CosmosDbContainerName = "YOUR_COSMOS_DB_CONTAINER_NAME";

        private Container GetContainerClient()
        {
            var cosmosClient = new CosmosClient(CosmosDbAccountUri, CosmosDbAccountPrimaryKey);
            var container = cosmosClient.GetContainer(CosmosDbDatabaseName, CosmosDbContainerName);
            return container;
        }        
        public async Task DeleteEngineer(string id, string partitionKey)
        {
            try
            {
                var container = GetContainerClient();
                var response = await container.DeleteItemAsync<Engineer>(id, new PartitionKey(partitionKey));
                Console.Write(response.StatusCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting engineer: {ex.Message}");
                throw;
            }
        }
        public async Task<List<Engineer>> GetEngineers()
        {
            List<Engineer> engineers = new List<Engineer>();
            try
            {
                var container = GetContainerClient();
                var query = "SELECT * FROM c";
                QueryDefinition queryDefinition = new QueryDefinition(query);
                FeedIterator<Engineer> feedIterator = container.GetItemQueryIterator<Engineer>(queryDefinition);
                while (feedIterator.HasMoreResults)
                {
                    FeedResponse<Engineer> response = await feedIterator.ReadNextAsync();
                    engineers.AddRange(response);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting engineers: {ex.Message}");
                throw;
            }
            return engineers;
        }
        public async Task<Engineer> GetEngineerById(string id, string partitionKey)
        {
            try
            {
                var container = GetContainerClient();
                ItemResponse<Engineer> response = await container.ReadItemAsync<Engineer>(id, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting engineer: {ex.Message}");
                throw;
            }
        }

        public async Task UsertEngineer(Engineer engineer)
        {
            try
            {
                if (engineer.id == null || engineer.id == Guid.Empty)
                {
                    engineer.id = Guid.NewGuid();
                }                
                var container = GetContainerClient();
                var response = await container.UpsertItemAsync(engineer, new PartitionKey(engineer.id.ToString()));
                Console.Write(response.StatusCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding engineer: {ex.Message}");
                throw;
            }
        }
    }
}
