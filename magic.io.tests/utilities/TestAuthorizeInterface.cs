/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using magic.io.contracts;

internal class TestAuthorizeInterface : IAuthorize
{
    bool IAuthorize.Authorize(string path, string username, string[] roles, AccessType type)
    {
        return false;
    }
}
