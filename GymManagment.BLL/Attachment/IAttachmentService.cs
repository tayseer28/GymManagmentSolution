using GymManagment.BLL.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.BLL.Attachment
{
    public interface IAttachmentService
    {
        //Task<Result> UploadAsync(IFormFile file);
        Task<Result<string>> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default);
        Result Delete(string fileName, string folderName);
        (Stream Stream, string ContentType)? GetFile(string fileName, string folderName);
    }
}
