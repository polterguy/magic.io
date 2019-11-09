/*
 * Magic, Copyright(c) Thomas Hansen 2019, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using magic.io.contracts;

namespace magic.io.services.utilities
{
    /*
     * Utility helper class for services.
     */
    internal class Utilities
    {
        readonly IAuthorize _authorize;

        public Utilities(IConfiguration configuration, IAuthorize authorize)
        {
            _authorize = authorize;

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            Root = (configuration["io:root-folder"] ?? "~/files")
                .Replace("~", Directory.GetCurrentDirectory())
                .TrimEnd('/') + "/";
        }

        public string Root { get; private set; }

        public string GetRelativePath(string absolute)
        {
            if (absolute.IndexOf(Root, StringComparison.InvariantCulture) != 0)
                throw new ArgumentOutOfRangeException($"'{absolute}' is not an absolute path and hence cannot be relativized");

            return "/" + absolute.Substring(Root.Length);
        }

        public string GetFullPath(string path = null, bool isFolder = false)
        {
            if (isFolder)
                return (Root + path?.Trim('/')).TrimEnd('/') + "/";
            return Root + path?.TrimStart('/');
        }

        public string GetMimeType(string filename)
        {
            if (filename.EndsWith(".hl", StringComparison.InvariantCulture))
                return "text/plain";
            return MimeTypes.GetMimeType(filename);
        }

        public bool HasAccess(
            string path,
            string username,
            string[] roles,
            AccessType type)
        {
            return _authorize?.Authorize(path, username, roles, type) ?? true;
        }
    }
}
