/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System.IO;
using System.Linq;
using System.Security;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using magic.io.contracts;
using www = magic.io.web.model;

namespace magic.io.services
{
    public class IOService : IIOService
    {
        #region [ -- Interface implementations -- ]

        public IEnumerable<www.Folder> GetFolders(
            string path,
            string username,
            string[] roles)
        {
            if (!HasAccess(path, username, roles))
                throw new SecurityException("Access denied");

            return Directory.GetDirectories(path).Select(x => new www.Folder
            {
                Path = x,
            });
        }

        public IEnumerable<www.File> GetFiles(
            string path,
            string username,
            string[] roles)
        {
            if (!HasAccess(path, username, roles))
                throw new SecurityException("Access denied");

            return Directory.GetFiles(path).Select(x => new www.File
            {
                Path = x,
            });
        }

        public FileResult GetFile(
            string path,
            string username,
            string[] roles)
        {
            if (!HasAccess(path, username, roles))
                throw new SecurityException("Access denied");

            return new FileStreamResult(File.OpenRead(path), GetContentType(path));
        }

        #endregion

        #region [ -- Private methods -- ]

        string GetContentType(string filename)
        {
            // TODO: Implement NuGet package for MIME types.
            switch(new FileInfo(filename).Extension)
            {
                case ".png":
                    return "image/png";
                default:
                    return "application/x-octetstream";
            }
        }

        bool HasAccess(
            string path,
            string username,
            string[] roles)
        {
            // TODO: Implement your own role based security checks here!
            return true;
        }

        #endregion
    }
}
