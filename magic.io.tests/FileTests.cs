/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
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
using magic.signals.services;
using magic.signals.contracts;
using magic.io.services.authorization;

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
        public void UploadDownloadHyperlambda()
        {
            var controller = CreateController();
            var fileMock = CreateMoqFile("foo", "test.hl");
            controller.Upload(fileMock.Object, "/");

            var result = controller.Download("/test.hl");
            Assert.Equal("test.hl", result.FileDownloadName);
            var fileStreamResult = result as FileStreamResult;
            using (fileStreamResult.FileStream)
            {
                var reader = new StreamReader(fileStreamResult.FileStream);
                Assert.Equal("foo", reader.ReadToEnd());
            }
        }

        [Fact]
        public void UploadThrows_01()
        {
            var controller = CreateController();

            // Empty file, should throw
            var fileMock = CreateMoqFile("", "test.txt");
            Assert.Throws<ArgumentException>(() => controller.Upload(fileMock.Object, "/"));
        }

        [Fact]
        public void UploadThrows_02()
        {
            var controller = CreateController();

            // Empty file, should throw
            Assert.Throws<NullReferenceException>(() => controller.Upload(null, "/"));
        }

        [Fact]
        public void UploadDownloadThrows_01()
        {
            var controller = CreateController();
            var fileMock = CreateMoqFile("File content", "test.txt");
            controller.Upload(fileMock.Object, "/");

            Assert.Throws<FileNotFoundException>(() => controller.Download("/test2.txt"));
        }

        [Fact]
        public void UploadCustomAuthorizeSlot()
        {
            var controller = CreateController(typeof(AuthorizeSlot));
            var fileMock = CreateMoqFile("File content", "foo.txt");
            controller.Upload(fileMock.Object, "/");
        }

        [Fact]
        public void UploadAuthorizeOnlyRoles_Throws()
        {
            var controller = CreateController(typeof(AuthorizeOnlyRoles));
            var fileMock = CreateMoqFile("File content", "foo.txt");
            Assert.Throws<SecurityException>(() => controller.Upload(fileMock.Object, "/"));
        }

        [Fact]
        public void UploadAuthorizeLambda()
        {
            var controller = CreateController(typeof(AuthorizeLambda));
            var fileMock = CreateMoqFile("File content", "foo.txt");
            controller.Upload(fileMock.Object, "/");
        }

        [Fact]
        public void UploadAuthorizeLambda_Throws()
        {
            var controller = CreateController(typeof(AuthorizeLambda));
            var fileMock = CreateMoqFile("File content", "error.txt");
            Assert.Throws<SecurityException>(() => controller.Upload(fileMock.Object, "/"));
        }

        [Fact]
        public void UploadCustomAuthorizeSlot_THROWS()
        {
            var controller = CreateController(typeof(AuthorizeSlot));

            /*
             * Notice, the slot [magic.io.authorize] which is a mock slot in this project,
             * throws if the filename doesn't end with 'foo.txt'.
             */
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
            var controller = CreateController(typeof(TestAuthorizeInterface));
            var fileMock = CreateMoqFile("File content", "test.txt");
            Assert.Throws<SecurityException>(() => controller.Upload(fileMock.Object, "/"));
        }

        [Fact]
        public void Authorized_Fail_02()
        {
            var controller = CreateController();
            var fileMock = CreateMoqFile("File content", "test.txt");
            controller.Upload(fileMock.Object, "/");

            controller = CreateController(typeof(TestAuthorizeInterface));
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

        FilesController CreateController(Type authType = null)
        {
            var services = new ServiceCollection();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<FilesController>();

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x[It.IsAny<string>()]).Returns("~/");
            services.AddTransient((svc) => mockConfiguration.Object);
            services.AddTransient<ISignaler, Signaler>();
            var types = new SignalsProvider(InstantiateAllTypes<ISlot>(services));
            services.AddTransient<ISignalsProvider>((svc) => types);

            if (authType == typeof(AuthorizeOnlyRoles))
                services.AddSingleton<IAuthorize>((svc) => new AuthorizeOnlyRoles("foo"));
            else if (authType == typeof(AuthorizeLambda))
                services.AddSingleton<IAuthorize>(new AuthorizeLambda((path, username, roles, accessType) => {
                    if (path.EndsWith("error.txt"))
                        return false;
                    return true;
                }));
            else if (authType != null)
                services.AddTransient(typeof(IAuthorize), authType);


            var provider = services.BuildServiceProvider();
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
