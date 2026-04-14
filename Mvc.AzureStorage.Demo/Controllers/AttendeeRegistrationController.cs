using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvc.AzureStorage.Demo.Data;
using Mvc.AzureStorage.Demo.Models;
using Mvc.AzureStorage.Demo.Services;

namespace Mvc.AzureStorage.Demo.Controllers
{
    public class AttendeeRegistrationController(ITableStorageServcie tableStorageServcie, 
        IBlobStorageService blobStorageService,
        IQueueService queueService) : Controller
    {
        private readonly ITableStorageServcie _tableStorageServcie = tableStorageServcie;
        private readonly IBlobStorageService _blobStorageService = blobStorageService;
        private readonly IQueueService _queueService = queueService;

        // GET: AttendeeRegistrationController
        public async Task<ActionResult> Index()
        {
            var data = await _tableStorageServcie.GetAttendeesAsync();
            foreach (var attendee in data)
            {
                attendee.ImageName = await _blobStorageService.GetBlobUrl(attendee.ImageName);
            }
            return View(data);
        }

        // GET: AttendeeRegistrationController/Details/5
        public async Task<ActionResult> Details(string industry, string id)
        {
            var data = await _tableStorageServcie.GetAttendeeAsync(industry, id);
            data.ImageName = await _blobStorageService.GetBlobUrl(data.ImageName);
            return View(data);
        }

        // GET: AttendeeRegistrationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AttendeeRegistrationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AttendeeEntity attendeeEntity, IFormFile formFile)
        {
            try
            {
                var id = Guid.NewGuid().ToString();
                attendeeEntity.PartitionKey=attendeeEntity.Industry;
                attendeeEntity.RowKey = id;
                if(formFile?.Length > 0) 
                {
                    attendeeEntity.ImageName = await _blobStorageService.UploadBlob(formFile, id);
                }
                else
                {
                    attendeeEntity.ImageName = "default.jpg";
                }
                await _tableStorageServcie.UpsertAttendeeAsync(attendeeEntity);
                var email = new EmailMessage
                {
                    EmailAddress = attendeeEntity.Email,
                    TimeStamp = DateTime.UtcNow,
                    Message = $"Hello {attendeeEntity.FirstName} {attendeeEntity.LastName}," +
                    $"\n\r Thank you for registering for this event. " +
                    $"\n\r Your record has been saved for future reference. "
                };
                await _queueService.SendMessage(email);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AttendeeRegistrationController/Edit/5
        public async Task<ActionResult> Edit(string industry, string id)
        {
            var data = await _tableStorageServcie.GetAttendeeAsync(industry, id);
            return View(data);
        }

        // POST: AttendeeRegistrationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AttendeeEntity attendeeEntity, IFormFile formFile)
        {
            try
            {
                if (formFile?.Length > 0)
                {
                    attendeeEntity.ImageName = await _blobStorageService.UploadBlob(formFile, attendeeEntity.RowKey);
                }
                attendeeEntity.PartitionKey = attendeeEntity.Industry;                
                await _tableStorageServcie.UpsertAttendeeAsync(attendeeEntity);
                var email = new EmailMessage
                {
                    EmailAddress = attendeeEntity.Email,
                    TimeStamp = DateTime.UtcNow,
                    Message = $"Hello {attendeeEntity.FirstName} {attendeeEntity.LastName}," +
                    $"\n\r Your record was modified successfully"
                };
                await _queueService.SendMessage(email);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: AttendeeRegistrationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string industry, string id)
        {
            try
            {
                var data = await _tableStorageServcie.GetAttendeeAsync(industry, id);
                await _tableStorageServcie.DeleteAttendeeAsync(industry, id);
                await _blobStorageService.RemoveBlob(data.ImageName);
                var email = new EmailMessage
                {
                    EmailAddress = data.Email,
                    TimeStamp = DateTime.UtcNow,
                    Message = $"Hello {data.FirstName} {data.LastName}," +
                   $"\n\r Your record was removed successfully"
                };
                await _queueService.SendMessage(email);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
