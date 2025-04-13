using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace IOInfoExtensions.TestUtilities
{
    public static class FileHelper
    {
        public static void WriteFiles(DirectoryInfo directory, string[] fileNames)
        {
            directory.Create();
            foreach (var fileName in fileNames)
            {
                var path = Path.Combine(directory.FullName, fileName);
                StreamWriter writer = null;
                try
                {
                    writer = new StreamWriter(path);
                    writer.WriteLine(path);
                }
                finally
                {
                    writer?.Close();
                    writer?.Dispose();
                }
            }

            GC.Collect();
        }

        public static bool HaveSameHash(FileInfo left, FileInfo right) =>
            GetHash(left) == GetHash(right);

        public static string GetHash(FileInfo file)
        {
            if (!file.Exists)
            {
                return string.Empty;
            }

            SHA1 hasher = null;
            StreamReader reader = null;

            try
            {
                hasher = SHA1.Create();
                reader = new StreamReader(file.FullName);
                var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(reader.ReadToEnd()));
                var builder = new StringBuilder(hash.Length * 2);

                foreach (var b in hash)
                {
                    _ = builder.Append(b.ToString("X2"));
                }

                return builder.ToString();
            }
            finally
            {
                reader?.Close();
                reader?.Dispose();
                hasher?.Dispose();
                GC.Collect();
            }
        }
    }
}
