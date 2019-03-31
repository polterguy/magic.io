/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System.Collections.Generic;
using magic.io.contracts;
using magic.io.web.model;

namespace magic.io.services
{
    public class IOService : IIOService
    {
        public IEnumerable<Folder> GetFolders(string path)
        {
            return null;
        }
    }
}
