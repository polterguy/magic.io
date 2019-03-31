/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System.IO;
using System.Linq;
using System.Collections.Generic;
using magic.io.contracts;
using www = magic.io.web.model;

namespace magic.io.services
{
    public class IOService : IIOService
    {
        #region [ -- Interface implementations -- ]

        public IEnumerable<www.Folder> GetFolders(string path)
        {
            return Directory.GetDirectories(path).Select(x => new www.Folder
            {
                Path = x,
            });
        }

        public IEnumerable<www.File> GetFiles(string path)
        {
            return Directory.GetFiles(path).Select(x => new www.File
            {
                Path = x,
            });
        }

        #endregion
    }
}
