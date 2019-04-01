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

namespace magic.io.services
{
    public class IOService : IIOService
    {
        readonly IConfiguration _configuration;

        public IOService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        #region [ -- Interface implementations -- ]

        public IEnumerable<www.Folder> GetFolders(
            string path,
            string username,
            string[] roles)
        {
            path = GetRelativePath(path);
            if (!HasAccess(
                path,
                username,
                roles,
                AccessType.ReadFolder,
                Directory.Exists(path)))
                throw new SecurityException("Access denied");

            return Directory.GetDirectories(path).Select(x => new www.Folder
            {
                Path = StripAbsolutePath(x),
            });
        }

        public IEnumerable<www.File> GetFiles(
            string path,
            string username,
            string[] roles)
        {
            path = GetRelativePath(path);
            if (!HasAccess(
                path,
                username,
                roles,
                AccessType.ReadFolder,
                Directory.Exists(path)))
                throw new SecurityException("Access denied");

            return Directory.GetFiles(path).Select(x => new www.File
            {
                Path = StripAbsolutePath(x),
            });
        }

        public void DeleteFolder(string path, string username, string[] roles)
        {
            path = GetRelativePath(path);
            if (!HasAccess(
                path,
                username,
                roles,
                AccessType.DeleteFolder,
                Directory.Exists(path)))
                throw new SecurityException("Access denied");

            Directory.Delete(path);
        }

        public void CreateFolder(string path, string username, string[] roles)
        {
            path = GetRelativePath(path);
            if (!HasAccess(
                path,
                username,
                roles,
                AccessType.WriteFolder,
                Directory.Exists(path)))
                throw new SecurityException("Access denied");

            Directory.CreateDirectory(path);
        }

        public FileResult GetFile(
            string path,
            string username,
            string[] roles)
        {
            path = GetRelativePath(path);
            if (!HasAccess(
                path,
                username,
                roles,
                AccessType.ReadFile,
                File.Exists(path)))
                throw new SecurityException("Access denied");

            return new FileStreamResult(File.OpenRead(path), GetContentType(path));
        }

        public void DeleteFile(string path, string username, string[] roles)
        {
            path = GetRelativePath(path);
            if (!HasAccess(
                path,
                username,
                roles,
                AccessType.DeleteFile,
                File.Exists(path)))
                throw new SecurityException("Access denied");

            File.Delete(path);
        }

        public void SaveFile(IFormFile file, string folder, string username, string[] roles)
        {
            if (file.Length <= 0)
                throw new ArgumentException("Empty file");

            folder = GetRelativePath(folder);
            var filename = folder + ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            if (!HasAccess(
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

        #endregion

        #region [ -- Private methods -- ]

        string StripAbsolutePath(string absolute)
        {
            var relative = GetRelativePath("/");
            return "/" + absolute.Substring(relative.Length).TrimStart('/');
        }

        string GetRelativePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var rootFolder = _configuration["io:root-folder"] ?? "~/files/";
            rootFolder = rootFolder.Replace("~", Directory.GetCurrentDirectory());
            rootFolder = rootFolder.TrimEnd('/') + "/";
            var fullPath = rootFolder + path.TrimStart('/');
            return fullPath;
        }

        string GetContentType(string filename)
        {
            return MimeTypes.GetMimeType(filename);
        }

        bool HasAccess(
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

        #endregion
    }
}
