/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using magic.io.contracts;
using magic.io.web.model;

namespace magic.io.web.controller
{
    /// <summary>
    /// IO controller for manipulating files and folders on your server
    /// </summary>
    [Route("api/folders")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class FoldersController : ControllerBase
    {
        readonly IFolderService _folderService;

        /// <summary>
        /// Creates a new instance of your folders controller
        /// </summary>
        /// <param name="service">Service containing business logic</param>
        public FoldersController(IFolderService service)
        {
            _folderService = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Returns all folders inside of the specified folder
        /// </summary>
        /// <param name="folder">Folder to return folders from within</param>
        /// <returns>List all folders inside of the specified folder</returns>
        [HttpGet]
        [Route("list-folders")]
        public ActionResult<IEnumerable<Folder>> GetFolders(string folder)
        {
            return Ok(_folderService.GetFolders(
                folder ?? "/",
                User.Identity.Name,
                User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray()));
        }

        /// <summary>
        /// Returns all files inside of the specified folder
        /// </summary>
        /// <param name="folder">Folder to return files from within</param>
        /// <returns>List all files inside of the specified folder</returns>
        [HttpGet]
        [Route("list-files")]
        public ActionResult<IEnumerable<File>> GetFiles(string folder)
        {
            return Ok(_folderService.GetFiles(
                folder ?? "/",
                User.Identity.Name,
                User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray()));
        }

        /// <summary>
        /// Deletes the specified folder
        /// </summary>
        /// <param name="folder">Folder to delete</param>
        [HttpDelete]
        public ActionResult DeleteFolder([Required] string folder)
        {
            _folderService.Delete(
                folder,
                User.Identity.Name,
                User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray());
            return Ok();
        }

        /// <summary>
        /// Creates the specified folder
        /// </summary>
        /// <param name="folder">Folder to create</param>
        [HttpPut]
        public ActionResult CreateFolder([Required] string folder)
        {
            _folderService.Create(
                folder,
                User.Identity.Name,
                User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray());
            return Ok();
        }
    }
}
