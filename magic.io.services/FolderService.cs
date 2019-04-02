/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using magic.io.contracts;
using magic.io.services.utilities;

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

        public IEnumerable<string> GetFolders(
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

            return Directory.GetDirectories(path)
                .Select(x => _utilities.GetRelativePath(x));
        }

        public IEnumerable<string> GetFiles(
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

            return Directory.GetFiles(path)
                .Select(x => _utilities.GetRelativePath(x));
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

        public void Move(string source, string destination, string username, string[] roles)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(destination))
                throw new ArgumentNullException(nameof(destination));

            source = _utilities.GetFullPath(source);
            if (!Directory.Exists(source))
                throw new ArgumentException($"Folder '{source}' does not exist");

            if (!_utilities.HasAccess(
                source,
                username,
                roles,
                AccessType.ReadFolder,
                Directory.Exists(source)))
                throw new SecurityException("Access denied");
            if (!_utilities.HasAccess(
                source,
                username,
                roles,
                AccessType.DeleteFolder,
                Directory.Exists(source)))
                throw new SecurityException("Access denied");

            destination = _utilities.GetFullPath(destination);
            if (Directory.Exists(destination))
                throw new ArgumentException($"Folder '{destination}' already exists");

            if (!_utilities.HasAccess(
                destination,
                username,
                roles,
                AccessType.WriteFolder,
                File.Exists(destination)))
                throw new SecurityException("Access denied");

            Directory.Move(source, destination);
        }

        #endregion
    }
}
