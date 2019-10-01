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
    /// <summary>
    /// Folder service implementation for listing folder's content, deleting folders, moving folders, etc.
    /// </summary>
    public class FolderService : IFolderService
    {
        readonly Utilities _utilities;

        /// <summary>
        /// Creates a new instance of your service implementation class.
        /// </summary>
        /// <param name="configuration">Configuration object for your server.</param>
        /// <param name="services">Service provider to retrieve services.</param>
        public FolderService(IConfiguration configuration, IServiceProvider kernel)
        {
            _utilities = new Utilities(configuration, kernel);
        }

        #region [ -- Interface implementations -- ]

        /// <summary>
        /// List folders inside of specified folder.
        /// </summary>
        /// <param name="path">Path to folder to list folders within.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        /// <returns>List of folders within your folder.</returns>
        public IEnumerable<string> ListFolders(
            string path,
            string username,
            string[] roles)
        {
            path = _utilities.GetFullPath(path, true);
            if (!_utilities.HasAccess(
                path,
                username,
                roles,
                AccessType.ReadFolder))
                throw new SecurityException("Access denied");

            return Directory.GetDirectories(path)
                .Select(x => _utilities.GetRelativePath(x) + "/");
        }

        /// <summary>
        /// List files inside of specified folder.
        /// </summary>
        /// <param name="path">Path to folder to list files within.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        /// <returns>List of files within your folder.</returns>
        public IEnumerable<string> ListFiles(
            string path,
            string username,
            string[] roles)
        {
            path = _utilities.GetFullPath(path, true);
            if (!_utilities.HasAccess(
                path,
                username,
                roles,
                AccessType.ReadFolder))
                throw new SecurityException("Access denied");

            return Directory.GetFiles(path)
                .Where(x => !x.EndsWith(".DS_Store")) // Removing Mac special files.
                .Select(_utilities.GetRelativePath);
        }

        /// <summary>
        /// Deletes some specified folder on your server.
        /// </summary>
        /// <param name="path">Path to folder to delete.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        public void Delete(string path, string username, string[] roles)
        {
            path = _utilities.GetFullPath(path, true);
            if (!_utilities.HasAccess(
                path,
                username,
                roles,
                AccessType.DeleteFolder))
                throw new SecurityException("Access denied");

            Directory.Delete(path, true);
        }

        /// <summary>
        /// Create some specified folder on your server.
        /// </summary>
        /// <param name="path">Path to folder to create.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        public void Create(string path, string username, string[] roles)
        {
            path = _utilities.GetFullPath(path, true);
            if (!_utilities.HasAccess(
                path,
                username,
                roles,
                AccessType.WriteFolder))
                throw new SecurityException("Access denied");

            if (Directory.Exists(path))
                Directory.Delete(path, true);

            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Moves some folder on your server to a new location.
        /// </summary>
        /// <param name="source">Your folder's current path.</param>
        /// <param name="destination">Destination for your folder.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        public void Move(string source, string destination, string username, string[] roles)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(destination))
                throw new ArgumentNullException(nameof(destination));

            source = _utilities.GetFullPath(source, true);
            if (!Directory.Exists(source))
                throw new ArgumentOutOfRangeException($"Folder '{source}' does not exist");

            if (!_utilities.HasAccess(
                source,
                username,
                roles,
                AccessType.ReadFolder))
                throw new SecurityException("Access denied");
            if (!_utilities.HasAccess(
                source,
                username,
                roles,
                AccessType.DeleteFolder))
                throw new SecurityException("Access denied");

            destination = _utilities.GetFullPath(destination, true);
            if (Directory.Exists(destination))
                Directory.Delete(destination, true);

            if (!_utilities.HasAccess(
                destination,
                username,
                roles,
                AccessType.WriteFolder))
                throw new SecurityException("Access denied");

            Directory.Move(source, destination);
        }

        /// <summary>
        /// Checks if some folder exists on your server.
        /// </summary>
        /// <param name="path">Path to folder.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        /// <returns>True if folder exists.</returns>
        public bool FolderExists(string path, string username, string[] roles)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            path = _utilities.GetFullPath(path, true);

            if (!_utilities.HasAccess(
                path,
                username,
                roles,
                AccessType.ReadFolder))
                throw new SecurityException("Access denied");

            return Directory.Exists(path);
        }

        /// <summary>
        /// Checks if some file exists on your server.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        /// <returns>True if file exists.</returns>
        public bool FileExists(string path, string username, string[] roles)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            path = _utilities.GetFullPath(path, false);

            if (!_utilities.HasAccess(
                path,
                username,
                roles,
                AccessType.ReadFile))
                throw new SecurityException("Access denied");

            return File.Exists(path);
        }

        #endregion
    }
}
