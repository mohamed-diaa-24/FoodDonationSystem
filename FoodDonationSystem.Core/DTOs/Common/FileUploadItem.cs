using Microsoft.AspNetCore.Http;

namespace FoodDonationSystem.Core.DTOs.Common
{
    public class FileUploadItem
    {
        public IFormFile File { get; set; }
        public string Folder { get; set; }
    }
}
