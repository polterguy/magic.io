/*
 * Magic, Copyright(c) Thomas Hansen 2019, thomas@gaiasoul.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using System.Linq;

namespace magic.io.tests.utilities
{
    [Slot(Name = "magic.io.authorize")]
    class AuthoriseSlot : ISlot
    {
        public void Signal(ISignaler signaler, Node input)
        {
            if (input.Children.First(x => x.Name == "path").Get<string>().EndsWith("foo.txt"))
                input.Value = true;
            else
                input.Value = false;
        }
    }
}
