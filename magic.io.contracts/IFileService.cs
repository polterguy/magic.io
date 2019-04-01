/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using magic.io.web.model;

namespace magic.io.contracts
{
    public interface IFileService
    {
        FileResult Get(string path, string username, string[] roles);

        void Delete(string path, string username, string[] roles);

        void Save(IFormFile file, string folder, string username, string[] roles);
    }
}
