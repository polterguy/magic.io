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
using magic.io.contracts;
using magic.io.services.utilities;

namespace magic.io.services
{
    public class FileService : IFileService
    {
        readonly Utilities _utilities;

        public FileService(IConfiguration configuration)
        {
            _utilities = new Utilities(configuration);
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

            return new FileStreamResult(
                File.OpenRead(path),
                _utilities.GetContentType(path));
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
            var filename = folder + ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
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
            if (File.Exists(destination))
                throw new ArgumentException($"File '{destination}' already exists");

            if (!_utilities.HasAccess(
                destination,
                username,
                roles,
                AccessType.WriteFile,
                File.Exists(source)))
                throw new SecurityException("Access denied");

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
            if (File.Exists(destination))
                throw new ArgumentException($"File '{destination}' already exists");

            if (!_utilities.HasAccess(
                destination,
                username,
                roles,
                AccessType.WriteFile,
                File.Exists(destination)))
                throw new SecurityException("Access denied");

            File.Move(source, destination);
        }

        #endregion
    }
}
