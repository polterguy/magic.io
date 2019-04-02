﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.Linq;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using magic.io.contracts;
using www = magic.io.web.model;

namespace magic.io.web.controller
{
    /// <summary>
    /// IO controller for manipulating files and folders on your server
    /// </summary>
    [Route("api/files")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class FilesController : ControllerBase
    {
        readonly IFileService _service;

        /// <summary>
        /// Creates a new instance of your files controller
        /// </summary>
        /// <param name="service">Service containing business logic</param>
        public FilesController(IFileService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Returns the specified file to caller
        /// </summary>
        /// <param name="file">File to return</param>
        /// <returns>The specified file</returns>
        [HttpGet]
        public FileResult Download([Required] string file)
        {
            return _service.Get(
                file,
                User.Identity.Name,
                User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray());
        }

        /// <summary>
        /// Uploads a file to your server and stores it at the specified path
        /// </summary>
        /// <param name="file">The actual file</param>
        /// <param name="folder">The folder on your server where you want to store your file</param>
        /// <returns>200 if file was successfully saved</returns>
        [HttpPost]
        [DisableRequestSizeLimit]
        [Consumes("multipart/form-data")]
        public ActionResult Upload([Required] [FromForm] IFormFile file, string folder)
        {
            _service.Save(
                file,
                folder ?? "/",
                User.Identity.Name,
                User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray());
            return Ok();
        }

        /// <summary>
        /// Deletes the specified file
        /// </summary>
        /// <param name="file">File to delete</param>
        [HttpDelete]
        public ActionResult DeleteFile([Required] string file)
        {
            _service.Delete(
                file,
                User.Identity.Name,
                User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray());
            return Ok();
        }

        /// <summary>
        /// Copies the specified source file to its given destination
        /// </summary>
        /// <param name="input">Source and destination file</param>
        [HttpPost]
        [Route("copy")]
        public ActionResult Copy([Required] www.CopyMoveModel input)
        {
            _service.Copy(
                input.Source,
                input.Destination,
                User.Identity.Name,
                User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray());
            return Ok();
        }

        /// <summary>
        /// Moves the specified source file to its given destination
        /// </summary>
        /// <param name="input">Source and destination file</param>
        [HttpPost]
        [Route("move")]
        public ActionResult Move([Required] www.CopyMoveModel input)
        {
            _service.Move(
                input.Source,
                input.Destination,
                User.Identity.Name,
                User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray());
            return Ok();
        }
    }
}
