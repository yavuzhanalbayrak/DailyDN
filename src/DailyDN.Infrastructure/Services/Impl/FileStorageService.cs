using Microsoft.AspNetCore.Http;
using DailyDN.Infrastructure.Models;
using Microsoft.Extensions.Options;
using System.Security;

namespace DailyDN.Infrastructure.Services.Impl
{
    public class FileStorageService(IOptions<FileStorageSettings> settings) : IFileStorageService
    {
        private readonly string _basePath = settings.Value.BasePath;
        private readonly string _baseUrl = settings.Value.BaseUrl;

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf", ".doc", ".docx", ".txt"
        };

        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        public bool FileExists(string filePath) => File.Exists(filePath);

        public Task DeleteFileAsync(string filePath)
        {
            var fullBasePath = Path.GetFullPath(_basePath);
            var fullFilePath = Path.GetFullPath(filePath);

            if (fullFilePath.StartsWith(fullBasePath, StringComparison.OrdinalIgnoreCase) && File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }

            return Task.CompletedTask;
        }

        public async Task<string> SaveFileAsync(string folderPath, IFormFile file, bool overwrite = false)
        {
            ValidateFile(file, AllowedExtensions);
            var targetFolder = GetSafeTargetFolder(folderPath);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(targetFolder, fileName);

            if (FileExists(filePath) && !overwrite)
                throw new InvalidOperationException("File already exists.");

            await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await file.CopyToAsync(stream);

            return BuildFileUrl(folderPath, fileName);
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
            var targetFolder = GetSafeTargetFolder(folderPath);

            if (Directory.Exists(targetFolder))
                Directory.Delete(targetFolder, true);

            Directory.CreateDirectory(targetFolder);

            return await SaveFilesAsync(folderPath, newFiles, overwrite: true);
        }

        public async Task<string> SaveProfilePhotoAsync(string userId, IFormFile file)
        {
            ValidateFile(file, AllowedImageExtensions);

            var folderPath = $"profiles/{userId}";
            var targetFolder = GetSafeTargetFolder(folderPath);

            if (Directory.Exists(targetFolder))
                Directory.Delete(targetFolder, recursive: true);

            Directory.CreateDirectory(targetFolder);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"profile{extension}";
            var filePath = Path.Combine(targetFolder, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await file.CopyToAsync(stream);

            return BuildFileUrl(folderPath, fileName);
        }

        private string GetSafeTargetFolder(string folderPath)
        {
            var fullBasePath = Path.GetFullPath(_basePath);
            var fullTargetFolder = Path.GetFullPath(Path.Combine(_basePath, folderPath));

            if (!fullTargetFolder.StartsWith(fullBasePath, StringComparison.OrdinalIgnoreCase))
                throw new SecurityException("Invalid target directory path (Path Traversal detected).");

            if (!Directory.Exists(fullTargetFolder))
                Directory.CreateDirectory(fullTargetFolder);

            return fullTargetFolder;
        }

        private static void ValidateFile(IFormFile file, HashSet<string> allowedExtensions)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be empty.", nameof(file));

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension))
                throw new SecurityException($"File extension '{extension}' is not allowed.");
        }

        private string BuildFileUrl(string folderPath, string fileName)
        {
            var baseUrlTrimmed = _baseUrl.TrimEnd('/');
            var relativePath = $"{folderPath.Trim('/')}/{fileName}".Replace("\\", "/");
            return $"{baseUrlTrimmed}/{relativePath}";
        }
    }
}
