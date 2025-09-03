using FoodDonationSystem.Core.DTOs.Common;
using Microsoft.AspNetCore.Http;

namespace FoodDonationSystem.Core.Interfaces.IServices
{
    public interface IFileService
    {
        Task<string?> SaveFileAsync(IFormFile? file, string folderName);
        Task<List<string>> SaveFilesWithRollbackAsync(List<FileUploadItem> files);
        void DeleteFiles(List<string> filePaths);
    }
}
