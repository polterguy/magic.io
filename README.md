
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

Before you can start consumption of the project, you'll need to think about what type of authorization process
you intend to use. The project contains three different authorization services. The most basic one accepts a
list of roles during construction, and could probably be consumed as a Singleton, and will only allow any type
of file access to a user belonging to one of the specified roles. To use this most simplest authorization
implementation, you could provide something like the following.

```csharp
/*
 * Somewhere were you initialize your ServiceProvider.
 *
 * This will only allow "admin" and "root" users to upload files to your server.
 */
service.AddTransient<IAuthorize>((svc) => new AuthorizeOnlyRoles("root", "admin"));
```

The slightly more advanced only, allows you to create your own C# slot, named __[magic.io.authorize]__,
that allows you to declare a slot in your C# code, using the syntax from magic.signals, which will
be given the path, username, and all other data necessary to determine whether or not you'd like to give
access to some resource or not. This allows you to control access to files according to the paths, file
extensions, etc.

The third one, allows you to declare your own dynamic __[magic.io.authorize]__ slot, as a __[slot]__ invocation
in your own Hyperlambda code, which will be given the same set of arguments as the above one, and be expected
to return either true or false, declaring whether or not the user has access to the requested resource or not.

In addition you can of course create your own authorization class, entirely in C#, and plug it into your
`ServiceProvider`, making sure the services will use your service when some resource is requested.
If you do, your `IAuthorize` implementation will be invoked every time some file resource is somehow
requested by the library for some reasons.

By default no authorization is given, unless you explicitly wire up some authorization service, using
for instance the ServiceProvider in your own wiring code.

## License

Magic is licensed as Affero GPL. This means that you can only use it to create Open Source solutions.
If this is a problem, you can contact at thomas@gaiasoul.com me to negotiate a proprietary license if
you want to use the framework to build closed source code. This will allow you to use Magic in closed
source projects, in addition to giving you access to Microsoft SQL Server adapters, to _"crudify"_
database tables in MS SQL Server. I also provide professional support for clients that buys a
proprietary enabling license.

