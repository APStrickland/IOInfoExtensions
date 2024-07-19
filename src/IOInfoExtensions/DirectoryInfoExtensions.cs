using System;
using System.IO;
using System.Linq;

namespace IOInfoExtensions
{
    /// <include file='..\Docs.xml' path='XmlCommentDocs/IOInfoExtensions/DirectoryInfoExtensions/Class/*' />
    public static class DirectoryInfoExtensions
    {
        /// <include file='..\Docs.xml' path='XmlCommentDocs/IOInfoExtensions/DirectoryInfoExtensions/Members/Member[@name="GetDirectory"]/*' />
        public static DirectoryInfo GetDirectory(this DirectoryInfo directory, string name, bool resolve = false, bool ignoreCase = false)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The name of the child directory cannot be null or empty.", nameof(name));
            }

            if (!string.IsNullOrWhiteSpace(Path.GetPathRoot(name)))
            {
                throw new ArgumentException("The name of the child directory cannot contain a root.", nameof(name));
            }

            name = name.TrimStart('.').Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            FileSystemInfo matchingChild = null;

            if (directory.Exists)
            {
                // See if a child at any nested level exists with the same leaf name. Then try to match the desired
                // path. This allows for nested directories that don't exist.
                matchingChild = Array.Find(directory.GetFileSystemInfos(Path.GetFileName(name), SearchOption.AllDirectories), x =>
                    x.FullName
                        .Remove(0, directory.FullName.Length)
                        .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                        .Equals(name, StringComparison.InvariantCultureIgnoreCase));
            }

            #pragma warning disable IDE0046 // Convert to conditional expression - Leaving as is for readability and clarity
            if (!ignoreCase && matchingChild?.Name.Equals(name, StringComparison.InvariantCulture) == false)
            {
                throw new DirectoryNotFoundException($"A child named '{name}' already exists but with a different case: {matchingChild.Name}.");
            }

            if (matchingChild?.Attributes.HasFlag(FileAttributes.Directory) == false)
            {
                throw new DirectoryNotFoundException($"A child named '{name}' already exists but is not a Directory.");
            }

            if (resolve && matchingChild == null)
            {
                throw new DirectoryNotFoundException($"Cannot find child '{name}' because it does not exist and resolve was set to true.");
            }
            #pragma warning restore IDE0046 // Convert to conditional expression

            return matchingChild != null
                ? new DirectoryInfo(matchingChild.FullName)
                : new DirectoryInfo(Path.Combine(directory.FullName, name));
        }

        /// <include file='..\Docs.xml' path='XmlCommentDocs/IOInfoExtensions/DirectoryInfoExtensions/Members/Member[@name="GetFile"]/*' />
        public static FileInfo GetFile(this DirectoryInfo directory, string name, bool resolve = false, bool ignoreCase = false)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The name of the child file cannot be null or empty.", nameof(name));
            }

            if (!string.IsNullOrWhiteSpace(Path.GetPathRoot(name)))
            {
                throw new ArgumentException("The name of the child file cannot contain a root.", nameof(name));
            }

            name = name.TrimStart('.').Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            FileSystemInfo matchingChild = null;

            if (directory.Exists)
            {
                // See if a child at any nested level exists with the same leaf name. Then try to match the desired
                // path. This allows for wildcards and nested directories that don't exist
                matchingChild = Array.Find(directory.GetFileSystemInfos(Path.GetFileName(name), SearchOption.AllDirectories), x =>
                    x.FullName
                        .Remove(0, directory.FullName.Length)
                        .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                        .Equals(name, StringComparison.InvariantCultureIgnoreCase));
            }

            var relativePath = matchingChild?.FullName.Remove(0, directory.FullName.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            #pragma warning disable IDE0046 // Convert to conditional expression - Leaving as is for readability and clarity
            if (!ignoreCase && matchingChild != null && !relativePath.Equals(name, StringComparison.InvariantCulture))
            {
                throw new FileNotFoundException($"A child named '{name}' already exists but with a different case: {relativePath}.");
            }

            if (matchingChild?.Attributes.HasFlag(FileAttributes.Directory) == true)
            {
                throw new FileNotFoundException($"A child named '{name}' already exists but is not a File.");
            }

            if (resolve && matchingChild == null)
            {
                throw new FileNotFoundException($"Cannot find child '{name}' because it does not exist and resolve was set to true.");
            }
            #pragma warning restore IDE0046 // Convert to conditional expression

            return matchingChild != null
                ? new FileInfo(matchingChild.FullName)
                : new FileInfo(Path.Combine(directory.FullName, name));
        }

        /// <include file='..\Docs.xml' path='XmlCommentDocs/IOInfoExtensions/DirectoryInfoExtensions/Members/Member[@name="DeleteContent"]/*' />
        public static void DeleteContent(this DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                return;
            }

            directory.GetDirectories().ToList().ForEach(dir => dir.Delete(true));
            directory.GetFiles().ToList().ForEach(file => file.Delete());
            directory.Refresh();
        }

        /// <include file='..\Docs.xml' path='XmlCommentDocs/IOInfoExtensions/DirectoryInfoExtensions/Members/Member[@name="CopyContentTo"]/*' />
        public static void CopyContentTo(this DirectoryInfo source, DirectoryInfo destination, bool copyEmptyDirectories = false, bool overwrite = false, bool cleanTarget = false)
        {
            // If the source directory doesn't exist there is nothing to do.
            if (!source.Exists)
            {
                return;
            }

            // If the target exists and Clean was specified, delete all the target content.
            if (destination.Exists && cleanTarget)
            {
                destination.DeleteContent();
            }

            // Loop on all the files and copy them
            foreach (var file in source.GetFiles("*", SearchOption.AllDirectories))
            {
                var relativePath = file.FullName.Substring(source.FullName.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                var destFile = new FileInfo(Path.Combine(destination.FullName, relativePath));
                destFile.Directory?.Create(); // If the directory already exists, this method does nothing.
                _ = file.CopyTo(destFile.FullName, overwrite);
            }

            // If CopyEmptyDirectories was specified, loop on the source directories and create them at the target
            if (copyEmptyDirectories)
            {
                foreach (var dir in source.GetDirectories("*", SearchOption.AllDirectories))
                {
                    var relativePath = dir.FullName.Substring(source.FullName.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    _ = Directory.CreateDirectory(Path.Combine(destination.FullName, relativePath));
                }
            }

            destination.Refresh();
        }
    }
}
