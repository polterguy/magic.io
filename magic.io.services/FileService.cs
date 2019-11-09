/*
 * Magic, Copyright(c) Thomas Hansen 2019, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
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
        /// Creates a new instance of your service implementation class without any
        /// authorization service implementation.
        /// </summary>
        /// <param name="configuration">Configuration object for your server.</param>
        public FileService(IConfiguration configuration)
        {
            _utilities = new Utilities(configuration, null);
        }

        /// <summary>
        /// Creates a new instance of your service implementation class with the specified
        /// authorize service implementation.
        /// </summary>
        /// <param name="configuration">Configuration object for your server.</param>
        /// <param name="authorize">Service provider to retrieve services.</param>
        public FileService(IConfiguration configuration, IAuthorize authorize)
        {
            _utilities = new Utilities(configuration, authorize ?? throw new ArgumentNullException(nameof(authorize)));
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
                File.Delete(filename);

            using (var stream = File.Create(filename))
            {
                file.CopyTo(stream);
            }
        }

        #endregion
    }
}
