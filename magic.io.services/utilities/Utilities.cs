/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using magic.io.contracts;
using www = magic.io.web.model;

namespace magic.io.services.utilities
{
    public class Utilities
    {
        public Utilities(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            Root = (configuration["io:root-folder"] ?? "~/files/")
                .Replace("~", Directory.GetCurrentDirectory())
                .TrimEnd('/') + "/";
        }

        public string Root { get; private set; }

        public string GetRelativePath(string absolute)
        {
            if (absolute.IndexOf(Root) != 0)
                throw new ArgumentException($"'{absolute}' is not an absolute path and hence cannot be relativized");

            return "/" + absolute.Substring(Root.Length);
        }

        public string GetFullPath(string path = null)
        {
            return Root + path?.TrimStart('/');
        }

        public string GetContentType(string filename)
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
