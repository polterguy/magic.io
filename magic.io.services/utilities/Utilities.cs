/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace magic.io.services.utilities
{
    public class Utilities
    {
        public Utilities(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            Root = configuration["io:root-folder"]
                .Replace("~", Directory.GetCurrentDirectory())
                .TrimEnd('/') + "/";
        }

        public string Root { get; private set; }

        public string GetRelativePath(string absolute)
        {
            if (absolute.IndexOf(Root) != 0)
                throw new ArgumentOutOfRangeException($"'{absolute}' is not an absolute path and hence cannot be relativized");

            return "/" + absolute.Substring(Root.Length);
        }

        public string GetFullPath(string path = null)
        {
            return Root + path?.TrimStart('/');
        }

        public string GetMimeType(string filename)
        {
            return MimeTypes.GetMimeType(filename);
        }

        public bool HasAccess(
            string path,
            string username,
            string[] roles,
            AccessType type,
            bool fileObjectExists)
        {
            /*
             * TODO: Implement your own role based security checks here!
             * You can fine grain access right according to path and access type,
             * and also whether or not the file/folder exists from before
             */
            return true;
        }
    }
}
