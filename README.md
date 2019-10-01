
# Magic IO for .Net

[![Build status](https://travis-ci.org/polterguy/magic.io.svg?master)](https://travis-ci.org/polterguy/magic.io)

A generic file/folder controller for .Net giving you access to the most important parts from `System.IO` through
an ASP.NET Web API. This controller allows you to download files, move files (on your server), upload files, and
save and delete files on your server, through a JSON based Web API.

## Authorization

The project contains three different authorization services. The most basic one accepts a list of roles during
construction, and could probably be consumed as a Singleton, and will only allow any type of file access to
a user belonging to one of the specified roles.

The slightly more advanced onle, allows you to create your own dynamic slot, named __[magic.io.authorize]__,
that allows you to declare a dynamic slot in your C# code, using the syntax from magic.signals, which will
be given the path, username, and all other data necessary to determine whether or not you'd like to give
access to some resource or not.

The third one, allows you to declare your own dynamic __[magic.io.authorize]__ slot, as a __[slot]__ invocation
in your own Hyperlambda code, which will be given the same set of arguments as the above one, and be expected
to return either true or false, declaring whether or not the user has access to the requested resource or not.

In addition you can of course create your own authorization class, entirely in C#, and plug it into your
`ServiceProvider`, making sure the services will use your service when some resource is requested.

By default no authorization is given, unless you explicitly wire up some authorization service, using
for instance the ServiceProvider in your own wiring code.

## License

Magic is licensed as Affero GPL. This means that you can only use it to create Open Source solutions.
If this is a problem, you can contact at thomas@gaiasoul.com me to negotiate a proprietary license if
you want to use the framework to build closed source code. This will allow you to use Magic in closed
source projects, in addition to giving you access to Microsoft SQL Server adapters, to _"crudify"_
database tables in MS SQL Server. I also provide professional support for clients that buys a
proprietary enabling license.

