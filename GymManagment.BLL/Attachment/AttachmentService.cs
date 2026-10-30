using GymManagementBLL.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.BLL.Attachment
{
    public class AttachmentService : IAttachmentService
    {
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5 MB
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };

        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AttachmentService> _logger;

        public AttachmentService(IWebHostEnvironment env, ILogger<AttachmentService> logger)

        {
            _env = env;
            _logger = logger;

        }
        public Result Delete(string fileName, string folderName)
        {

            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName)) return Result.NotFound("File not Found");

            try
            {
                var fullPath = Path.Combine(_env.WebRootPath, folderName, fileName);

                if (!File.Exists(fullPath)) return Result.NotFound("Path Not Found");
                File.Delete(fullPath);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete attachment {File}.", fileName);
                return Result.Fail("Failed to Delete File");
            }
        }

        public async Task<Result<string>> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default)
        {
            if (fileStream is null || !fileStream.CanRead) return Result<string>.NotFound("File Stream Not Found or Cannot be Read");


            if (fileStream.Length == 0) return Result<string>.Fail("File is Empty");
            if (fileStream.Length > _maxFileSize)
            {
                _logger.LogWarning("Rejected upload: file too large ({Size} bytes).", fileStream.Length);
                return Result<string>.Fail("Rejected upload: file too large (Max Size is 5 MB)");
            }

            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("Rejected upload: extension {Ext} not allowed.", extension);
                return Result<string>.Fail("Rejected upload: only jpg, png, jpej extensions are allowed.");
            }

            var uploadsFolder = Path.Combine(_env.ContentRootPath, folderName); // get the path of the uploads folder in my project 
                                                                                // D:\what to keep\route\Back End\8. MVC\session 01\GymManagmentSolution\GymManagment\MembersPhoto\
            Directory.CreateDirectory(uploadsFolder);

            var storedFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, storedFileName); // D:\what to keep\route\Back End\8. MVC\session 01\GymManagmentSolution\GymManagment\MembersPhoto\Test.png

            try // un managed code so we put it in try catch 
            {
                await using var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                await fileStream.CopyToAsync(fs, ct);
                return Result<string>.Ok(storedFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload file {FileName}.", fileName);
                return Result<string>.Fail("Failed to upload file");
            }
        }

        public (Stream Stream, string ContentType)? GetFile(string fileName, string folderName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName)) return null;

            var fullPath = Path.Combine(_env.ContentRootPath, folderName, fileName);

            if (!File.Exists(fullPath)) return null;

            var contentType = Path.GetExtension(fullPath).ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream" //Binary data 
            };

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read); // we dont use using keyword cause we dont want to close the stream manually
            return (stream, contentType);
        }
    }
}
