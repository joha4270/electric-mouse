using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace electric_mouse.Services
{
    public static class FileExtentions
    {
        /// <summary>
        /// Gets a random file name and preserves the original file extension
        /// </summary>
        public static string GetRandomFileNameWithOriginalExtension(this string fileName) => $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}{Path.GetExtension(fileName)}";

        /// <summary>
        /// Checks if the filename has one of the extensions provided.
        /// </summary>
        public static bool HasExtension(this string fileName, params string[] extensions)
        {
            string fileExtension = Path.GetExtension(fileName);

            return extensions.Any(extension => extension.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the input megabytes in bytes
        /// </summary>
        public static long ConvertMegabytesToBytes(this long megabytes) => megabytes * 1024L * 1024L;
    }
}
