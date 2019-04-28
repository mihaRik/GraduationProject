using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Extensions
{
    public static class FileExtension
    {
        public static bool IsPhoto(this IFormFile file)
        {
            return file.ContentType == "image/jpg" ||
                   file.ContentType == "image/jpeg" ||
                   file.ContentType == "image/gif" ||
                   file.ContentType == "image/png";
        }

        public async static Task<string> SavePhotoAsync(this IFormFile photo, string root, string path)
        {
            var fileName = Path.Combine("/images", path, Guid.NewGuid().ToString() + Path.GetFileName(photo.FileName));
            var fileFullPath = root + @"\" + fileName;

            using (var stream = new FileStream(fileFullPath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            return fileName;
        }
    }
}
