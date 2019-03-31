
/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using Microsoft.Extensions.Configuration;
using Ninject;
using magic.io.contracts;
using magic.common.contracts;

namespace magic.io.services.init
{
    class ConfigureNinject : IConfigureNinject
    {
        public void Configure(IKernel kernel, IConfiguration configuration)
        {
            kernel.Bind<IIOService>().To<IOService>();
        }
    }
}
