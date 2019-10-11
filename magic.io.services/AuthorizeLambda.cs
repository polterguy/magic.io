/*
 * Magic, Copyright(c) Thomas Hansen 2019, thomas@gaiasoul.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using magic.io.contracts;

namespace magic.io.services
{
    /// <summary>
    /// Authorization service consumed when some file/folder resource is being accessed.
    /// Will be invoked to determine if the user has access to the resource or not.
    /// 
    /// This service implementation requires a lambda callback that will be invoked
    /// whenever access to some resource is requested
    /// </summary>
    public class AuthorizeLambda : IAuthorize
    {
        readonly Func<string, string, string[], AccessType, bool> _functor;

        /// <summary>
        /// Create a new instance of your authorization service.
        /// </summary>
        public AuthorizeLambda(Func<string, string, string[], AccessType, bool> functor)
        {
            _functor = functor ?? throw new ArgumentNullException(nameof(functor));
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
            return _functor(path, username, roles, type);
        }
    }
}
