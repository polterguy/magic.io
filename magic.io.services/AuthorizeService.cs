/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System.Linq;
using magic.io.contracts;

namespace magic.io.services
{
    /// <summary>
    /// Authorization service consumed when some file/folder resource is being accessed.
    /// Will be invoked to determine if the user has access to the resource or not.
    /// </summary>
    public class AuthorizeService : IAuthorize
    {
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
            // TODO: Implement Hyperlambda slot callback here, to allow for overriding authorization more dynamically
            return roles.Any(x => x == "root");
        }
    }
}
