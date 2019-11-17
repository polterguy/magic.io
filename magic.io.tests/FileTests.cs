/*
 * Magic, Copyright(c) Thomas Hansen 2019, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using magic.io.services;
using magic.io.contracts;
using magic.io.controller;
using magic.signals.contracts;
using magic.signals.services;

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
            controller.Upload(fileMock.Object, "/");

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
        public void UploadCustomAuthorizeSlot()
        {
            var controller = CreateController(typeof(AuthorizeSlot));
            var fileMock = CreateMoqFile("File content", "foo.txt");
            controller.Upload(fileMock.Object, "/");
        }

        [Fact]
        public void UploadCustomAuthorizeSlot_THROWS()
        {
            var controller = CreateController(typeof(AuthorizeSlot));
            var fileMock = CreateMoqFile("File content", "bar.txt");
            Assert.Throws<SecurityException>(() => controller.Upload(fileMock.Object, "/"));
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
            controller.Upload(fileMock.Object, "/");

            fileMock = CreateMoqFile("File content 2", "test.txt");
            controller.Upload(fileMock.Object, "/");

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
        public void Authorized_Fail_01()
        {
            var controller = CreateController(typeof(Authorize));
            var fileMock = CreateMoqFile("File content", "test.txt");
            Assert.Throws<SecurityException>(() => controller.Upload(fileMock.Object, "/"));
        }

        [Fact]
        public void Authorized_Fail_02()
        {
            var controller = CreateController();
            var fileMock = CreateMoqFile("File content", "test.txt");
            controller.Upload(fileMock.Object, "/");

            controller = CreateController(typeof(Authorize));
            Assert.Throws<SecurityException>(() => controller.Download("/test.txt"));
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

        FilesController CreateController(Type authType = null)
        {
            var kernel = new ServiceCollection();
            kernel.AddTransient<IFileService, FileService>();
            kernel.AddTransient<FilesController>();

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x[It.IsAny<string>()]).Returns("~/");
            kernel.AddTransient((svc) => mockConfiguration.Object);
            kernel.AddTransient<ISignaler, Signaler>();
            var types = new SignalsProvider(InstantiateAllTypes<ISlot>(kernel));
            kernel.AddTransient<ISignalsProvider>((svc) => types);

            if (authType != null)
                kernel.AddTransient(typeof(IAuthorize), authType);

            var provider = kernel.BuildServiceProvider();
            return provider.GetService(typeof(FilesController)) as FilesController;
        }

        static IEnumerable<Type> InstantiateAllTypes<T>(ServiceCollection services) where T : class
        {
            var type = typeof(T);
            var result = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic && !x.FullName.StartsWith("Microsoft", StringComparison.InvariantCulture))
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            foreach (var idx in result)
            {
                services.AddTransient(idx);
            }
            return result;
        }

        #endregion
    }
}
