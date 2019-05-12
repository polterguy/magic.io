
/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using Microsoft.Extensions.Configuration;
using magic.io.contracts;
using magic.common.contracts;
using Microsoft.Extensions.DependencyInjection;

namespace magic.io.services.init
{
    class ConfigureServices : IConfigureServices
    {
        public void Configure(IServiceCollection kernel, IConfiguration configuration)
        {
            kernel.AddTransient<IFileService, FileService>();
            kernel.AddTransient<IFolderService, FolderService>();
        }
    }
}
