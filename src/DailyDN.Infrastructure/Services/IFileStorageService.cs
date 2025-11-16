using Microsoft.AspNetCore.Http;

namespace DailyDN.Infrastructure.Services
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Saves the given file to the specified folder path.
        /// </summary>
        Task<string> SaveFileAsync(string folderPath, IFormFile file, bool overwrite = false);

        /// <summary>
        /// Saves multiple files to the specified folder path.
        /// </summary>
        Task<IEnumerable<string>> SaveFilesAsync(string folderPath, IEnumerable<IFormFile> files, bool overwrite = false);

        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        Task DeleteFileAsync(string filePath);

        /// <summary>
        /// Checks if the specified file exists.
        /// </summary>
        bool FileExists(string filePath);
    }
}
