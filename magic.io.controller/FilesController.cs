﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.Linq;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using magic.io.contracts;

namespace magic.io.controller
{
    /// <summary>
    /// IO controller for manipulating files on your server.
    /// </summary>
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        readonly IFileService _service;

        /// <summary>
        /// Creates a new instance of your files controller.
        /// </summary>
        /// <param name="service">Service containing implementation.</param>
        public FilesController(IFileService service)
        {
            _service = service;
        }

        /// <summary>
        /// Uploads a file to your server and stores it at the specified path.
        /// </summary>
        /// <param name="file">The actual file.</param>
        /// <param name="folder">The folder on your server where you want to store your file.</param>
        [HttpPut]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(8000000)] // Maximum 8MB payloads are accepted
        public void Upload([Required] [FromForm] IFormFile file, string folder)
        {
            _service.Upload(
                file,
                folder ?? "/",
                User?.Identity.Name,
                User?.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray());
        }

        /// <summary>
        /// Downloads the specified file to client.
        /// </summary>
        /// <param name="file">File to download.</param>
        /// <returns>The specified file.</returns>
        [HttpGet]
        public FileResult Download([Required] string file)
        {
            if (Response != null) // Allowing for testing endpoint.
                Response.Headers["Access-Control-Expose-Headers"] = "Content-Disposition";
            return _service.Download(
                file,
                User?.Identity.Name,
                User?.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray());
        }
    }
}
