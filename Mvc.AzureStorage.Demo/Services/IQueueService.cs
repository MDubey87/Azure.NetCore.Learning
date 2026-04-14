using Mvc.AzureStorage.Demo.Models;

namespace Mvc.AzureStorage.Demo.Services
{
    public interface IQueueService
    {
        Task SendMessage(EmailMessage emailMessage);
    }
}