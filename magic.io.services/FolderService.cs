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
using magic.io.services.utilities;
using www = magic.io.web.model;

namespace magic.io.services
{
    public class FolderService : IFolderService
    {
        readonly Utilities _utilities;

        public FolderService(IConfiguration configuration)
        {
            _utilities = new Utilities(configuration);
        }

        #region [ -- Interface implementations -- ]

        public IEnumerable<www.Folder> GetFolders(
            string path,
            string username,
            string[] roles)
        {
            path = _utilities.GetFullPath(path);
            if (!_utilities.HasAccess(
                path,
                username,
                roles,
                AccessType.ReadFolder,
                Directory.Exists(path)))
                throw new SecurityException("Access denied");

            return Directory.GetDirectories(path).Select(x => new www.Folder
            {
                Path = _utilities.GetRelativePath(x),
            });
        }

        public IEnumerable<www.File> GetFiles(
            string path,
            string username,
            string[] roles)
        {
            path = _utilities.GetFullPath(path);
            if (!_utilities.HasAccess(
                path,
                username,
                roles,
                AccessType.ReadFolder,
                Directory.Exists(path)))
                throw new SecurityException("Access denied");

            return Directory.GetFiles(path).Select(x => new www.File
            {
                Path = _utilities.GetRelativePath(x),
            });
        }

        public void Delete(string path, string username, string[] roles)
        {
            path = _utilities.GetFullPath(path);
            if (!_utilities.HasAccess(
                path,
                username,
                roles,
                AccessType.DeleteFolder,
                Directory.Exists(path)))
                throw new SecurityException("Access denied");

            Directory.Delete(path);
        }

        public void Create(string path, string username, string[] roles)
        {
            path = _utilities.GetFullPath(path);
            if (!_utilities.HasAccess(
                path,
                username,
                roles,
                AccessType.WriteFolder,
                Directory.Exists(path)))
                throw new SecurityException("Access denied");

            Directory.CreateDirectory(path);
        }

        #endregion
    }
}
