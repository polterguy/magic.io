
# Magic IO

A generic file upload/download controller for .Net allowing you to upload and download files to and from your server.

## Usage

The project is intended to be dynamically added to your Web API as a controller, making sure you create
an association between the `IFileService` and its `FileService` by mapping it up as a transient service
using something such as follows for instance.

```csharp
/*
 * Somewhere were you initialize your ServiceProvider.
 */
service.AddTransient<IFileService, FileService>();
```

Then assuming you're able to dynamically add the _"magic.io.controller"_ as a controller endpoint plugin, you'll
end up with two endpoints as follows.

* `GET` - `api/files` - Downloads the _"file"_ QUERY parameter file. Remember to URL encode your QUERY parameters.
* `PUT` - `api/files` - Uploads the _"file_" file. Use _"multipart/form-data"_ as `Content-Type` for your request.

If you PUT a file that already exists, the existing file will be overwritten. However, you'll also need to think
about authorization before you actually start using the library.

## Authorization

Before you can start consuming the project, you'll need to think about what type of authorization process
you intend to use. The project contains 4 different authorization services. The most basic one accepts a
list of roles during construction, and could probably be consumed as a Singleton, and will only allow any type
of file access to a user belonging to one of the specified roles. To use the simplest authorization
implementation, you could provide something like the following, that will _only_ allow users belonging to the
_"admin"_ role, and the _"root"_ role to upload and download files.

```csharp
/*
 * Somewhere were you initialize your ServiceProvider.
 *
 * This will only allow "admin" and "root" users to upload files to your server.
 */
service.AddTransient<IAuthorize>((svc) => new AuthorizeOnlyRoles("root", "admin"));
```

The next step of authorization, is to use `AuthorizeLambda`, which allows you to supply a `Func`, that
will be invoked with the path, username, roles and `AccessType` (read/write) requested. This allows you to
create any amount of C# to verify the user is allowed to access the requested access type for the given
file, and return true or false from your function, depending upon whether or not you want to grant the
user access or not.

The slightly more advanced only, allows you to create your own C# slot, named __[magic.io.authorize]__,
that allows you to declare a slot in your C# code, using the syntax from magic.signals, which will
be given the path, username, and all other data necessary to determine whether or not you'd like to give
access to some resource or not. This allows you to control access to files according to the paths, file
extensions, etc.

The fourth authorization scheme, allows you to declare your own dynamic __[magic.io.authorize]__ slot, as
a __[slot]__ invocation in your own Hyperlambda code, which will be given the same set of arguments as
the above **[magic.io.authorize]** static slot, and be expected to return either true or false,
declaring whether or not the user has access to the requested resource or not.

Both of the latter methods will be given the following arguments.

* __[path]__ - Path to file requested by caller
* __[username]__ - Username of user trying to access file
* __[type]__ - Type of access requested by user (can be _"ReadFile"_ or _"WriteFile"_)
* __[roles]__ - Roles user belongs to

In addition you can of course create your own `IAuthorize` implementation, entirely in C#, and plug it into your
`ServiceProvider`, making sure the service will be resolved when some service within magic.io needs it.
If you do, your `IAuthorize` implementation will be invoked every time some file resource is somehow
requested by the library for some reasons.

By default no authorization is given, unless you explicitly wire up some authorization service, using
for instance the ServiceProvider in your own wiring code.

## Quality gates

- [![Build status](https://travis-ci.com/polterguy/magic.io.svg?master)](https://travis-ci.com/polterguy/magic.io)
- [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.io&metric=alert_status)](https://sonarcloud.io/dashboard?id=polterguy_magic.io)
- [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.io&metric=bugs)](https://sonarcloud.io/dashboard?id=polterguy_magic.io)
- [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.io&metric=code_smells)](https://sonarcloud.io/dashboard?id=polterguy_magic.io)
- [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.io&metric=coverage)](https://sonarcloud.io/dashboard?id=polterguy_magic.io)
- [![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.io&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=polterguy_magic.io)
- [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.io&metric=ncloc)](https://sonarcloud.io/dashboard?id=polterguy_magic.io)
- [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.lambda.io&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=polterguy_magic.lambda.io)
- [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.io&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=polterguy_magic.io)
- [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.io&metric=security_rating)](https://sonarcloud.io/dashboard?id=polterguy_magic.io)
- [![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.io&metric=sqale_index)](https://sonarcloud.io/dashboard?id=polterguy_magic.io)
- [![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.io&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=polterguy_magic.io)
