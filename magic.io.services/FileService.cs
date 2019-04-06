/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.IO;
using System.Security;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Ninject;
using magic.io.contracts;
using magic.io.services.utilities;

namespace magic.io.services
{
    public class FileService : IFileService
    {
        readonly Utilities _utilities;

        public FileService(IConfiguration configuration, IKernel kernel)
        {
            _utilities = new Utilities(configuration, kernel);
        }

        #region [ -- Interface implementations -- ]

        public FileResult Get(
            string path,
            string username,
            string[] roles)
        {
            path = _utilities.GetFullPath(path);
            if (!_utilities.HasAccess(
                path,
                username,
                roles,
                AccessType.ReadFile,
                File.Exists(path)))
                throw new SecurityException("Access denied");

            if (!File.Exists(path))
                throw new ArgumentOutOfRangeException("File doesn't exist");

            var result = new FileStreamResult(
                File.OpenRead(path),
                _utilities.GetMimeType(path));
            result.FileDownloadName = Path.GetFileName(path);
            return result;
        }

        public void Delete(string path, string username, string[] roles)
        {
            path = _utilities.GetFullPath(path);
            if (!_utilities.HasAccess(
                path,
                username,
                roles,
                AccessType.DeleteFile,
                File.Exists(path)))
                throw new SecurityException("Access denied");

            File.Delete(path);
        }

        public void Save(IFormFile file, string folder, string username, string[] roles)
        {
            if (file.Length <= 0)
                throw new ArgumentException("Empty file");

            folder = _utilities.GetFullPath(folder);
            var filename = folder + file.FileName;
            if (!_utilities.HasAccess(
                filename,
                username,
                roles,
                AccessType.WriteFile,
                File.Exists(folder)))
                throw new SecurityException("Access denied");

            using (var stream = File.Create(filename))
            {
                file.CopyTo(stream);
            }
        }

        public void Copy(string source, string destination, string username, string[] roles)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(destination))
                throw new ArgumentNullException(nameof(destination));

            source = _utilities.GetFullPath(source);
            if (!File.Exists(source))
                throw new ArgumentOutOfRangeException($"File '{source}' does not exist");

            if (!_utilities.HasAccess(
                source,
                username,
                roles,
                AccessType.ReadFile,
                File.Exists(source)))
                throw new SecurityException("Access denied");

            destination = _utilities.GetFullPath(destination);

            if (!_utilities.HasAccess(
                destination,
                username,
                roles,
                AccessType.WriteFile,
                File.Exists(source)))
                throw new SecurityException("Access denied");

            if (File.Exists(destination))
                File.Delete(destination);

            File.Copy(source, destination);
        }

        public void Move(string source, string destination, string username, string[] roles)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(destination))
                throw new ArgumentNullException(nameof(destination));

            source = _utilities.GetFullPath(source);
            if (!File.Exists(source))
                throw new ArgumentOutOfRangeException($"File '{source}' does not exist");

            if (!_utilities.HasAccess(
                source,
                username,
                roles,
                AccessType.ReadFile,
                File.Exists(source)))
                throw new SecurityException("Access denied");
            if (!_utilities.HasAccess(
                source,
                username,
                roles,
                AccessType.DeleteFile,
                File.Exists(source)))
                throw new SecurityException("Access denied");

            destination = _utilities.GetFullPath(destination);

            if (!_utilities.HasAccess(
                destination,
                username,
                roles,
                AccessType.WriteFile,
                File.Exists(destination)))
                throw new SecurityException("Access denied");

            if (File.Exists(destination))
                File.Delete(destination);

            File.Move(source, destination);
        }

        #endregion
    }
}
