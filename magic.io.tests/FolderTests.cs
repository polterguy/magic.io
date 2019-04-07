/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.IO;
using System.Linq;
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
    public class FolderTests
    {
        #region [ -- Unit tests -- ]

        [Fact]
        public void CreateFolderListFolders()
        {
            var controller = CreateController();
            AssertHelper.Single(controller.Create("foo"));
            var result = AssertHelper.List(controller.ListFolders("/"));
            Assert.True(result.Count() > 0);
            Assert.Contains("/foo", result);
        }

        [Fact]
        public void CreateFolderAndFileListFiles()
        {
            var controller = CreateController();
            AssertHelper.Single(controller.Create("foo"));
            var result = AssertHelper.List(controller.ListFolders("/"));
            var file = FileTests.CreateMoqFile("foo content", "foo.txt");
            var filesControllers = CreateFilesController();
            AssertHelper.Single(filesControllers.Upload(file.Object, "foo"));
            var files = AssertHelper.List(controller.ListFiles("foo"));
            Assert.Single(files);
            Assert.Equal("/foo/foo.txt", files.First());
        }

        [Fact]
        public void CreateFolderAndFileDeleteFileListFiles()
        {
            var controller = CreateController();
            AssertHelper.Single(controller.Create("foo"));
            var result = AssertHelper.List(controller.ListFolders("/"));
            var file = FileTests.CreateMoqFile("foo content", "foo.txt");
            var filesControllers = CreateFilesController();
            AssertHelper.Single(filesControllers.Upload(file.Object, "foo"));
            AssertHelper.Single(filesControllers.Delete("foo/foo.txt"));
            var files = AssertHelper.List(controller.ListFiles("foo"));
            Assert.Empty(files);
        }

        [Fact]
        public void CreateFolderMoveFolder()
        {
            var controller = CreateController();
            AssertHelper.Single(controller.Create("foo"));
            AssertHelper.Single(controller.Move(new CopyMoveModel
            {
                Source = "foo",
                Destination = "bar"
            }));
            var result = AssertHelper.List(controller.ListFolders("/"));
            Assert.True(result.Count() > 0);
            Assert.Contains("/bar", result);
            Assert.DoesNotContain("/foo", result);
        }

        [Fact]
        public void Authorized_Fail_01()
        {
            var controller = CreateController(true);
            Assert.Throws<SecurityException>(() => controller.Create("foo"));
        }

        [Fact]
        public void Authorized_Fail_02()
        {
            var controller = CreateController();
            AssertHelper.Single(controller.Create("foo"));

            controller = CreateController(true);
            Assert.Throws<SecurityException>(() => controller.ListFolders("/"));
        }

        [Fact]
        public void Authorized_Fail_03()
        {
            var controller = CreateController(true);
            Assert.Throws<SecurityException>(() => controller.ListFiles("/"));
        }

        [Fact]
        public void Authorized_Fail_04()
        {
            var controller = CreateController();
            AssertHelper.Single(controller.Create("foo"));

            controller = CreateController(true);
            Assert.Throws<SecurityException>(() => controller.Move(new CopyMoveModel
            {
                Source = "/foo",
                Destination = "/bar",
            }));
        }

        [Fact]
        public void Authorized_Fail_05()
        {
            var controller = CreateController();
            AssertHelper.Single(controller.Create("foo"));

            controller = CreateController(true);
            Assert.Throws<SecurityException>(() => controller.Delete("/foo"));
        }

        #endregion

        #region [ -- Private helper methods -- ]

        FilesController CreateFilesController()
        {
            var kernel = new StandardKernel();

            kernel.Bind<IFileService>().To<FileService>();
            kernel.Bind<FilesController>().ToSelf();

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x[It.IsAny<string>()]).Returns("~/");
            kernel.Bind<IConfiguration>().ToConstant(mockConfiguration.Object);

            return kernel.Get<FilesController>();
        }

        FoldersController CreateController(bool authorize = false)
        {
            var kernel = new StandardKernel();

            kernel.Bind<IFolderService>().To<FolderService>();
            kernel.Bind<FoldersController>().ToSelf();

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x[It.IsAny<string>()]).Returns("~/");
            kernel.Bind<IConfiguration>().ToConstant(mockConfiguration.Object);

            if (authorize)
            {
                kernel.Bind<IAuthorize>().To<FileTests.Authorize>();
            }

            return kernel.Get<FoldersController>();
        }

        #endregion
    }
}
