namespace Blazor.CosmosDB.Demo.Data
{
    public interface IEngineerService
    {
        Task DeleteEngineer(string id, string partitionKey);
        Task<List<Engineer>> GetEngineers();
        Task<Engineer> GetEngineerById(string id, string partitionKey);
        Task UsertEngineer(Engineer engineer);
    }
}