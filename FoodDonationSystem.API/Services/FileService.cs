using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.Interfaces.IServices;

namespace FoodDonationSystem.API.Services
{
    public class FileService : IFileService
    {

        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string?> SaveFileAsync(IFormFile? file, string folderName)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads", folderName);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var extension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine("/", "Uploads", folderName, uniqueFileName).Replace("\\", "/");
        }

        public async Task<List<string>> SaveFilesWithRollbackAsync(List<FileUploadItem> files)
        {
            var savedFiles = new List<string>();

            try
            {
                foreach (var file in files)
                {
                    var savedFilePath = await SaveFileAsync(file.File, file.Folder);
                    if (savedFilePath != null)
                        savedFiles.Add(savedFilePath);
                }
                return savedFiles;
            }
            catch
            {
                DeleteFiles(savedFiles);
                throw;
            }
        }

        public void DeleteFiles(List<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                var fullPath = Path.Combine(_env.WebRootPath, filePath);
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }
        }
    }
}
