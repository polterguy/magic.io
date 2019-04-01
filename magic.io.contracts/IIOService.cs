/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using magic.io.web.model;

namespace magic.io.contracts
{
    public interface IIOService
    {
        #region [ -- Folder methods -- ]

        IEnumerable<Folder> GetFolders(string path, string username, string[] roles);

        IEnumerable<File> GetFiles(string path, string username, string[] roles);

        void DeleteFolder(string path, string username, string[] roles);

        void CreateFolder(string path, string username, string[] roles);

        #endregion

        #region [ -- File methods -- ]

        FileResult GetFile(string path, string username, string[] roles);

        void DeleteFile(string path, string username, string[] roles);

        #endregion
    }
}
