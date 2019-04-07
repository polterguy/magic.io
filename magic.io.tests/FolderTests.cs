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
            Assert.True(result.Any(x => x == "/foo"));
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
            AssertHelper.Single(filesControllers.DeleteFile("foo/foo.txt"));
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
            Assert.True(result.Any(x => x == "/bar"));
            Assert.False(result.Any(x => x == "/foo"));
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

        FoldersController CreateController()
        {
            var kernel = new StandardKernel();

            kernel.Bind<IFolderService>().To<FolderService>();
            kernel.Bind<FoldersController>().ToSelf();

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x[It.IsAny<string>()]).Returns("~/");
            kernel.Bind<IConfiguration>().ToConstant(mockConfiguration.Object);

            return kernel.Get<FoldersController>();
        }

        #endregion
    }
}
