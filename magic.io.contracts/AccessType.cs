﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

namespace magic.io.contracts
{
    /// <summary>
    /// Type of access specified to Authorization service, allowing you to determine what type
    /// of access some client is trying to retrieve for some file/folder resources in your installation.
    /// </summary>
    public enum AccessType
    {
        /// <summary>
        /// Reading of file type of access required.
        /// </summary>
        ReadFile,

        /// <summary>
        /// Writing of file type of access required.
        /// </summary>
        WriteFile,
    }
}
