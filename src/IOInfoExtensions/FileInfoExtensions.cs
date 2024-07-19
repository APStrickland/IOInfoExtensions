using System.IO;

namespace IOInfoExtensions
{
    /// <include file='..\Docs.xml' path='XmlCommentDocs/IOInfoExtensions/FileInfoExtensions/Class/*' />
    public static class FileInfoExtensions
    {
        /// <include file='..\Docs.xml' path='XmlCommentDocs/IOInfoExtensions/FileInfoExtensions/Members/Member[@name="MoveFrom"]/*' />
        public static void MoveFrom(this FileInfo destination, FileInfo source, bool overwrite = false)
        {
            if (!source.Exists)
            {
                throw new FileNotFoundException($"The source file '{source.FullName}' does not exist.", source.FullName);
            }

            // If the destination exists and we are not overwriting, throw an exception.
            if (destination.Exists)
            {
                if (!overwrite)
                {
                    throw new IOException($"The destination file '{destination.FullName}' already exists and overwrite is not set to true.");
                }
                else
                {
                    destination.Delete();
                }
            }

            destination.Directory?.Create();
            var temp = new FileInfo(source.FullName);
            temp.MoveTo(destination.FullName);
            destination.Refresh();
            source.Refresh();
        }

        /// <include file='..\Docs.xml' path='XmlCommentDocs/IOInfoExtensions/FileInfoExtensions/Members/Member[@name="CopyFrom"]/*' />
        public static void CopyFrom(this FileInfo destination, FileInfo source, bool overwrite = false)
        {
            if (!source.Exists)
            {
                throw new FileNotFoundException($"The source file {source.FullName} does not exist.", source.FullName);
            }

            destination.Directory?.Create();
            _ = source.CopyTo(destination.FullName, overwrite);

            destination.Refresh();
            source.Refresh();
        }

        /// <include file='..\Docs.xml' path='XmlCommentDocs/IOInfoExtensions/FileInfoExtensions/Members/Member[@name="TryDelete"]/*' />
        public static void TryDelete(this FileInfo file)
        {
            if (file.Exists)
            {
                file.Delete();
                file.Refresh();
            }
        }
    }
}
