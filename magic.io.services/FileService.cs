/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.IO;
using System.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using magic.io.contracts;
using magic.io.services.utilities;

namespace magic.io.services
{
    /// <summary>
    /// File service implementation, giving you access to download, delete, upload, copy and move files
    /// on your server.
    /// </summary>
    public class FileService : IFileService
    {
        readonly Utilities _utilities;

        /// <summary>
        /// Creates a new instance of your service implementation class.
        /// </summary>
        /// <param name="configuration">Configuration object for your server.</param>
        /// <param name="services">Service provider to retrieve services.</param>
        public FileService(IConfiguration configuration, IServiceProvider services)
        {
            _utilities = new Utilities(configuration, services);
        }

        #region [ -- Interface implementations -- ]

        /// <summary>
        /// Downloads a file to the client.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        /// <returns>A file result.</returns>
        public FileResult Download(
            string path,
            string username,
            string[] roles)
        {
            path = _utilities.GetFullPath(path);
            if (!_utilities.HasAccess(
                path,
                username,
                roles,
                AccessType.ReadFile))
                throw new SecurityException("Access denied");

            if (!File.Exists(path))
                throw new FileNotFoundException($"File '{path}' does not exist");

            return new FileStreamResult(
                File.OpenRead(path),
                _utilities.GetMimeType(path))
            {
                FileDownloadName = Path.GetFileName(path)
            };
        }

        /// <summary>
        /// Deletes a file on your server.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        public void Delete(string path, string username, string[] roles)
        {
            path = _utilities.GetFullPath(path);
            if (!_utilities.HasAccess(
                path,
                username,
                roles,
                AccessType.DeleteFile))
                throw new SecurityException("Access denied");

            if (!File.Exists(path))
                throw new FileNotFoundException($"File '{path}' does not exist");

            File.Delete(path);
        }

        /// <summary>
        /// Uploads a file to the server.
        /// </summary>
        /// <param name="file">File that is attempted to be uploaded.</param>
        /// <param name="folder">Folder of where to save file.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        public void Upload(IFormFile file, string folder, string username, string[] roles)
        {
            if (file.Length <= 0)
                throw new ArgumentException($"File '{file.FileName}' is empty");

            var filename = _utilities.GetFullPath(folder, true) + file.FileName;
            if (!_utilities.HasAccess(
                filename,
                username,
                roles,
                AccessType.WriteFile))
                throw new SecurityException("Access denied");

            if (File.Exists(filename))
            {
                if (!_utilities.HasAccess(
                    filename,
                    username,
                    roles,
                    AccessType.DeleteFile))
                    throw new SecurityException("Access denied");

                File.Delete(filename);
            }

            using (var stream = File.Create(filename))
            {
                file.CopyTo(stream);
            }
        }

        /// <summary>
        /// Copies a file from one location to another on your server.
        /// </summary>
        /// <param name="source">Path to file you want to copy.</param>
        /// <param name="destination">Path to its new destination.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        public void Copy(string source, string destination, string username, string[] roles)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(destination))
                throw new ArgumentNullException(nameof(destination));

            source = _utilities.GetFullPath(source);
            if (!File.Exists(source))
                throw new FileNotFoundException($"File '{source}' does not exist");

            if (!_utilities.HasAccess(
                source,
                username,
                roles,
                AccessType.ReadFile))
                throw new SecurityException("Access denied");

            destination = _utilities.GetFullPath(destination);

            if (!_utilities.HasAccess(
                destination,
                username,
                roles,
                AccessType.WriteFile))
                throw new SecurityException("Access denied");

            if (File.Exists(destination))
            {
                if (!_utilities.HasAccess(
                    destination,
                    username,
                    roles,
                    AccessType.DeleteFile))
                    throw new SecurityException("Access denied");

                File.Delete(destination);
            }

            File.Copy(source, destination);
        }

        /// <summary>
        /// Moves a file from one location to another on your server.
        /// </summary>
        /// <param name="source">Path to file you want to move.</param>
        /// <param name="destination">Path to its new destination.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        public void Move(string source, string destination, string username, string[] roles)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(destination))
                throw new ArgumentNullException(nameof(destination));

            source = _utilities.GetFullPath(source);
            if (!File.Exists(source))
                throw new FileNotFoundException($"File '{source}' does not exist");

            if (!_utilities.HasAccess(
                source,
                username,
                roles,
                AccessType.ReadFile))
                throw new SecurityException("Access denied");

            if (!_utilities.HasAccess(
                source,
                username,
                roles,
                AccessType.DeleteFile))
                throw new SecurityException("Access denied");

            destination = _utilities.GetFullPath(destination);

            if (!_utilities.HasAccess(
                destination,
                username,
                roles,
                AccessType.WriteFile))
                throw new SecurityException("Access denied");

            if (File.Exists(destination))
            {
                if (!_utilities.HasAccess(
                    destination,
                    username,
                    roles,
                    AccessType.DeleteFile))
                    throw new SecurityException("Access denied");

                File.Delete(destination);
            }

            File.Move(source, destination);
        }

        #endregion
    }
}
