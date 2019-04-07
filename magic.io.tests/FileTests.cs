/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Ninject;
using magic.tests;
using magic.io.services;
using magic.io.web.model;
using magic.io.contracts;
using magic.io.web.controller;
using System.Security;

namespace magic.io.tests
{
    public class FileTests
    {
        #region [ -- Unit tests -- ]

        [Fact]
        public void UploadDownload()
        {
            var controller = CreateController();
            var fileMock = CreateMoqFile("File content", "test.txt");
            AssertHelper.Single(controller.Upload(fileMock.Object, "/"));

            var result = controller.Download("/test.txt");
            Assert.Equal("test.txt", result.FileDownloadName);
            var fileStreamResult = result as FileStreamResult;
            using (fileStreamResult.FileStream)
            {
                var reader = new StreamReader(fileStreamResult.FileStream);
                Assert.Equal("File content", reader.ReadToEnd());
            }
        }

        [Fact]
        public void UploadNonExistingFolder()
        {
            var controller = CreateController();
            var fileMock = CreateMoqFile("foo", "/non-existing-folder/test.txt");
            Assert.Throws<DirectoryNotFoundException>(() => controller.Upload(fileMock.Object, "/"));
        }

        [Fact]
        public void UploadOverwriteDownload()
        {
            var controller = CreateController();
            var fileMock = CreateMoqFile("File content 1", "test.txt");
            AssertHelper.Single(controller.Upload(fileMock.Object, "/"));

            fileMock = CreateMoqFile("File content 2", "test.txt");
            AssertHelper.Single(controller.Upload(fileMock.Object, "/"));

            var result = controller.Download("/test.txt");
            Assert.Equal("test.txt", result.FileDownloadName);
            var fileStreamResult = result as FileStreamResult;
            using (fileStreamResult.FileStream)
            {
                var reader = new StreamReader(fileStreamResult.FileStream);
                Assert.Equal("File content 2", reader.ReadToEnd());
            }
        }

        [Fact]
        public void UploadDeleteDownload_Fail()
        {
            var controller = CreateController();
            var fileMock = CreateMoqFile("File content", "test.txt");
            AssertHelper.Single(controller.Upload(fileMock.Object, "/"));

            AssertHelper.Single(controller.Delete("/test.txt"));

            Assert.Throws<FileNotFoundException>(() => controller.Download("/test.txt"));
        }

        [Fact]
        public void UploadCopyDownload()
        {
            var controller = CreateController();
            var fileMock = CreateMoqFile("File content", "test.txt");
            AssertHelper.Single(controller.Upload(fileMock.Object, "/"));

            AssertHelper.Single(controller.Copy(new CopyMoveModel
            {
                Source = "/test.txt",
                Destination = "/test2.txt",
            }));

            var result = controller.Download("/test2.txt");
            Assert.Equal("test2.txt", result.FileDownloadName);
            var fileStreamResult = result as FileStreamResult;
            using (fileStreamResult.FileStream)
            {
                var reader = new StreamReader(fileStreamResult.FileStream);
                Assert.Equal("File content", reader.ReadToEnd());
            }

            result = controller.Download("/test.txt");
            Assert.Equal("test.txt", result.FileDownloadName);
            fileStreamResult = result as FileStreamResult;
            using (fileStreamResult.FileStream)
            {
                var reader = new StreamReader(fileStreamResult.FileStream);
                Assert.Equal("File content", reader.ReadToEnd());
            }
        }

        [Fact]
        public void UploadMoveDownload()
        {
            var controller = CreateController();
            var fileMock = CreateMoqFile("File content", "test.txt");
            AssertHelper.Single(controller.Upload(fileMock.Object, "/"));

            AssertHelper.Single(controller.Move(new CopyMoveModel
            {
                Source = "/test.txt",
                Destination = "/test2.txt",
            }));

            var result = controller.Download("/test2.txt");
            Assert.Equal("test2.txt", result.FileDownloadName);
            var fileStreamResult = result as FileStreamResult;
            using (fileStreamResult.FileStream)
            {
                var reader = new StreamReader(fileStreamResult.FileStream);
                Assert.Equal("File content", reader.ReadToEnd());
            }

            Assert.Throws<FileNotFoundException>(() => controller.Download("/test.txt"));
        }

        [Fact]
        public void Authorized_Fail_01()
        {
            var controller = CreateController(true);
            var fileMock = CreateMoqFile("File content", "test.txt");
            Assert.Throws<SecurityException>(() => controller.Upload(fileMock.Object, "/"));
        }

        [Fact]
        public void Authorized_Fail_02()
        {
            var controller = CreateController();
            var fileMock = CreateMoqFile("File content", "test.txt");
            AssertHelper.Single(controller.Upload(fileMock.Object, "/"));

            controller = CreateController(true);
            Assert.Throws<SecurityException>(() => controller.Download("/test.txt"));
        }

        [Fact]
        public void Authorized_Fail_03()
        {
            var controller = CreateController();
            var fileMock = CreateMoqFile("File content", "test.txt");
            AssertHelper.Single(controller.Upload(fileMock.Object, "/"));

            controller = CreateController(true);
            Assert.Throws<SecurityException>(() => controller.Move(new CopyMoveModel
            {
                Source = "/test.txt",
                Destination = "/test2.txt",
            }));
        }

        [Fact]
        public void Authorized_Fail_04()
        {
            var controller = CreateController();
            var fileMock = CreateMoqFile("File content", "test.txt");
            AssertHelper.Single(controller.Upload(fileMock.Object, "/"));

            controller = CreateController(true);
            Assert.Throws<SecurityException>(() => controller.Copy(new CopyMoveModel
            {
                Source = "/test.txt",
                Destination = "/test2.txt",
            }));
        }

        [Fact]
        public void Authorized_Fail_05()
        {
            var controller = CreateController();
            var fileMock = CreateMoqFile("File content", "test.txt");
            AssertHelper.Single(controller.Upload(fileMock.Object, "/"));

            controller = CreateController(true);
            Assert.Throws<SecurityException>(() => controller.Delete("/test.txt"));
        }

        #endregion

        #region [ -- Private helper methods -- ]

        static internal Mock<IFormFile> CreateMoqFile(string content, string name)
        {
            var fileMock = new Mock<IFormFile> { CallBase = true };
            var fileName = name;
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Flush();
            ms.Position = 0;
            fileMock.Setup(x => x.FileName).Returns(fileName);
            fileMock.Setup(x => x.Length).Returns(ms.Length);
            fileMock.Setup(x => x.CopyTo(It.IsAny<Stream>())).Callback<Stream>(x => ms.CopyTo(x));
            return fileMock;
        }

        internal class Authorize : IAuthorize
        {
            bool IAuthorize.Authorize(string path, string username, string[] roles, AccessType type)
            {
                return false;
            }
        }

        FilesController CreateController(bool authorize = false)
        {
            var kernel = new StandardKernel();

            kernel.Bind<IFileService>().To<FileService>();
            kernel.Bind<FilesController>().ToSelf();

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x[It.IsAny<string>()]).Returns("~/");
            kernel.Bind<IConfiguration>().ToConstant(mockConfiguration.Object);

            if (authorize)
            {
                kernel.Bind<IAuthorize>().To<Authorize>();
            }

            return kernel.Get<FilesController>();
        }

        #endregion
    }
}
