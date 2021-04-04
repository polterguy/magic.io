/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.Linq;
using magic.node;
using magic.io.contracts;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.io.services.authorization
{
    /// <summary>
    /// Authorization service consumed when somee file/folder resource is being accessed.
    /// Will be invoked to determine if the user has access to the resource or not.
    /// 
    /// This service implementation will signal the dynamic slot called [magic.io.authorize] with its parameters,
    /// and only allow access if that slot returns true. Notice, you'll have to actually declare this
    /// slot somehow in your own C# code.
    /// </summary>
    public class AuthorizeDynamicSlot : IAuthorize
    {
        readonly ISignaler _signaler;

        /// <summary>
        /// Create a new instance of your authorization service.
        /// </summary>
        public AuthorizeDynamicSlot(ISignaler signaler)
        {
            _signaler = signaler;
        }

        /// <summary>
        /// Returns true if the user has access to the resource for the specified type of access requested.
        /// </summary>
        /// <param name="path">Path to resource access is requested for.</param>
        /// <param name="username">Username of user trying to access resource.</param>
        /// <param name="roles">Roles user belongs to.</param>
        /// <param name="type">Type of access requested.</param>
        /// <returns>True if user has access to perform action, otherwise false.</returns>
        public bool Authorize(string path, string username, string[] roles, AccessType type)
        {
            var pars = new Node("signal", "magic.io.authorize");
            pars.Add(new Node("path", path));
            pars.Add(new Node("username", username));
            pars.Add(new Node("type", type.ToString()));
            pars.Add(new Node("roles", null, roles.Select(x => new Node(null, x))));
            _signaler.Signal("signal", pars);
            return pars.Get<bool>();
        }
    }
}
