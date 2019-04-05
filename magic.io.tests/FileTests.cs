/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Ninject;
using magic.tests;
using magic.io.services;
using magic.io.contracts;
using magic.io.web.controller;

namespace magic.io.tests
{
    public class FileTests
    {
        #region [ -- Unit tests -- ]

        [Fact]
        public void UploadDownload()
        {
            var controller = CreateController();

            var fileMock = new Mock<IFormFile> { CallBase = true };
            var content = "Moq file for unit tests";
            var fileName = "test.txt";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Flush();
            ms.Position = 0;
            fileMock.Setup(x => x.OpenReadStream()).Returns(ms);
            fileMock.Setup(x => x.FileName).Returns(fileName);
            fileMock.Setup(x => x.Length).Returns(ms.Length);
            fileMock.Setup(x => x.CopyTo(It.IsAny<Stream>())).Callback<Stream>(x => ms.CopyTo(x));
            AssertHelper.Single(controller.Upload(fileMock.Object, "/"));

            // TODO: Implementing downloading to verify content of file.
        }

        #endregion

        #region [ -- Private helper methods -- ]

        FilesController CreateController()
        {
            var kernel = new StandardKernel();
            kernel.Bind<IFileService>().To<FileService>();
            kernel.Bind<FilesController>().ToSelf();
            var configuration = new ConfigurationBuilder().Build();
            kernel.Bind<IConfiguration>().ToConstant(configuration);
            return kernel.Get<FilesController>();
        }

        #endregion
    }
}
