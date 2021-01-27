using System.IO;

namespace IO
{
    public static class PersistentStreamUtils
    {
        public static Stream GetReadStream(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            return File.Exists(filePath) ? FileUtilities.GetReadStream(filePath) : null;
        }
    }
}