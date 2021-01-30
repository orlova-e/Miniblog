using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Web.Infrastructure.Extensions
{
    public static class IFormFileExtensions
    {
        /// <summary>
        /// Tries to update or create file on the specified path
        /// </summary>
        /// <param name="formFile">The IFormFile file to update of create</param>
        /// <param name="path">The path to the file</param>
        /// <returns>The path to the file, if it was updated or created, otherwise an empty string</returns>
        public static async Task<string> TryUpdateFileAsync(this IFormFile formFile, string path)
        {
            if (File.Exists(path) && !Path.GetFileName(path).Equals(formFile.FileName))
            {
                File.Delete(path);
            }

            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string newPath = Path.Combine(directory, formFile.FileName).Replace(@"\\", "/");

            try
            {
                using (FileStream fileStream = new FileStream(newPath, FileMode.Create, FileAccess.Write))
                {
                    await formFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return newPath;
        }

        /// <summary>
        /// Read bytes from the IFormFile stream
        /// </summary>
        /// <param name="formFile">The IFormFile file to read bytes from it</param>
        /// <returns>The bytes read from the stream</returns>
        public static byte[] ReadBytes(this IFormFile formFile)
        {
            byte[] file = new byte[] { };
            using (BinaryReader binaryReader = new BinaryReader(formFile.OpenReadStream()))
            {
                file = binaryReader.ReadBytes((int)formFile.Length);
            }
            return file;
        }
    }
}
