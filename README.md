
# Magic IO for .Net

[![Build status](https://travis-ci.org/polterguy/magic.io.svg?master)](https://travis-ci.org/polterguy/magic.io)

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

There is also a convenience authorization implementation, allowing you to simply provide a lambda
`Func` callback, that will be invoked whenever some resource is being accessed, or some file is uploaded.
This authorize service implementation class is called `AuthorizeLambda`.

By default no authorization is given, unless you explicitly wire up some authorization service, using
for instance the ServiceProvider in your own wiring code.

## License

Although most of Magic's source code is Open Source, you will need a license key to use it.
[You can obtain a license key here](https://servergardens.com/buy/).
Notice, 7 days after you put Magic into production, it will stop working, unless you have a valid
license for it.

* [Get licensed](https://servergardens.com/buy/)
