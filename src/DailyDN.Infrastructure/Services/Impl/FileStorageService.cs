using Microsoft.AspNetCore.Http;
using DailyDN.Infrastructure.Models;
using Microsoft.Extensions.Options;

namespace DailyDN.Infrastructure.Services.Impl
{
    public class FileStorageService(IOptions<FileStorageSettings> settings) : IFileStorageService
    {
        private readonly string _basePath = settings.Value.BasePath;
        private readonly string _baseUrl = settings.Value.BaseUrl;

        public bool FileExists(string filePath) => File.Exists(filePath);

        public async Task DeleteFileAsync(string filePath)
        {
            if (FileExists(filePath))
                File.Delete(filePath);
        }

        public async Task<string> SaveFileAsync(string folderPath, IFormFile file, bool overwrite = false)
        {
            var targetFolder = Path.Combine(_basePath, folderPath);

            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(targetFolder, fileName);

            if (FileExists(filePath) && !overwrite)
                throw new InvalidOperationException("File already exists.");

            await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await file.CopyToAsync(stream);

            return $"{_baseUrl}/{folderPath}/{fileName}".Replace("\\", "/");
        }

        public async Task<IEnumerable<string>> SaveFilesAsync(string folderPath, IEnumerable<IFormFile> files, bool overwrite = false)
        {
            var savedFiles = new List<string>();
            foreach (var file in files)
            {
                var url = await SaveFileAsync(folderPath, file, overwrite);
                savedFiles.Add(url);
            }
            return savedFiles;
        }

        public async Task<IEnumerable<string>> ReplaceFilesAsync(string folderPath, IEnumerable<IFormFile> newFiles)
        {
            var targetFolder = Path.Combine(_basePath, folderPath);

            if (Directory.Exists(targetFolder))
                Directory.Delete(targetFolder, true);

            Directory.CreateDirectory(targetFolder);

            return await SaveFilesAsync(folderPath, newFiles, overwrite: true);
        }

        public async Task<string> SaveProfilePhotoAsync(string userId, IFormFile file)
        {
            var folderPath = $"profiles/{userId}";
            var targetFolder = Path.Combine(_basePath, folderPath);

            if (Directory.Exists(targetFolder))
                Directory.Delete(targetFolder, recursive: true);

            Directory.CreateDirectory(targetFolder);

            var fileName = "profile.jpg";
            var filePath = Path.Combine(targetFolder, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await file.CopyToAsync(stream);

            return $"{_baseUrl}/{folderPath}/{fileName}".Replace("\\", "/");
        }

    }
}
