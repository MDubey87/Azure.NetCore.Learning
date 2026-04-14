using Mvc.AzureStorage.Demo.Data;

namespace Mvc.AzureStorage.Demo.Services
{
    public interface ITableStorageServcie
    {
        Task DeleteAttendeeAsync(string industry, string id);
        Task<AttendeeEntity> GetAttendeeAsync(string industry, string id);
        Task<List<AttendeeEntity>> GetAttendeesAsync();
        Task UpsertAttendeeAsync(AttendeeEntity attendee);
    }
}