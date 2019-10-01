/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System.Collections.Generic;

namespace magic.io.contracts
{
    /// <summary>
    /// Folder service interface for listing folder's content, deleting folders, moving folders, etc.
    /// </summary>
    public interface IFolderService
    {
        /// <summary>
        /// List folders inside of specified folder.
        /// </summary>
        /// <param name="path">Path to folder to list folders within.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        /// <returns>List of folders within your folder.</returns>
        IEnumerable<string> ListFolders(string path, string username, string[] roles);

        /// <summary>
        /// List files inside of specified folder.
        /// </summary>
        /// <param name="path">Path to folder to list files within.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        /// <returns>List of files within your folder.</returns>
        IEnumerable<string> ListFiles(string path, string username, string[] roles);

        /// <summary>
        /// Deletes some specified folder on your server.
        /// </summary>
        /// <param name="path">Path to folder to delete.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        void Delete(string path, string username, string[] roles);

        /// <summary>
        /// Create some specified folder on your server.
        /// </summary>
        /// <param name="path">Path to folder to create.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        void Create(string path, string username, string[] roles);

        /// <summary>
        /// Moves some folder on your server to a new location.
        /// </summary>
        /// <param name="source">Your folder's current path.</param>
        /// <param name="destination">Destination for your folder.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        void Move(string source, string destination, string username, string[] roles);

        /// <summary>
        /// Checks if some folder exists on your server.
        /// </summary>
        /// <param name="path">Path to folder.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        /// <returns>True if folder exists.</returns>
        bool FolderExists(string path, string username, string[] roles);

        // TODO: Consider moving this into IFileService
        /// <summary>
        /// Checks if some file exists on your server.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <param name="username">Username of user creating request.</param>
        /// <param name="roles">Roles user belongs to that is creating the request.</param>
        /// <returns>True if file exists.</returns>
        bool FileExists(string path, string username, string[] roles);
    }
}
