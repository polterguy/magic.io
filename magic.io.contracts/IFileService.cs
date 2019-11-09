/*
 * Magic, Copyright(c) Thomas Hansen 2019, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace magic.io.contracts
{
    /// <summary>
    /// File service interface, giving you access to download, delete, upload, copy and move files
    /// on your server.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Downloads a file to the client.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        /// <returns>A file result.</returns>
        FileResult Download(string path, string username, string[] roles);

        /// <summary>
        /// Uploads a file to the server.
        /// </summary>
        /// <param name="file">File that is attempted to be uploaded.</param>
        /// <param name="folder">Folder of where to save file.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        void Upload(IFormFile file, string folder, string username, string[] roles);
    }
}
