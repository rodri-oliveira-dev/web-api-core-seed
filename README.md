Legacy preservation notice
==========================

> **Attention:** this repository currently preserves a legacy ASP.NET Core Web API built for .NET Core 3.1. .NET Core 3.1 reached end of support on December 13, 2022.

This code should not be used as the base for new projects. It is being preserved as historical reference only, so later modernization work can compare against the original behavior without changing the legacy baseline. A modernization to .NET 10 is planned for a later phase.

At the end of Phase 1, the preserved legacy version will be identified by the tag `v1.0.0-legacy` and by the branch `legacy/netcoreapp3.1`. These Git references are planned but are not created yet in this delivery.

See [LEGACY.md](LEGACY.md) for the documented legacy requirements, commands, migrations, seed status, limitations, and validation notes.

What is the Project?
=====================
The objective of this project was to implement the most commonly used technologies, and to share as a base project for WEB API in NET Core 3.1

## Give a Star! :star:
If you liked the project or if project helped you, please give a star ;)

## How to use:
- You will need the latest Visual Studio 2019 and the latest .NET Core SDK.
- ***Please check if you have installed the same runtime version (SDK) described in global.json***
- The latest SDK and tools can be downloaded from https://dot.net/core.

Also you can run the Project in Visual Studio Code (Windows, Linux or MacOS).

To know more about how to setup your enviroment visit the [Microsoft .NET Download Guide](https://www.microsoft.com/net/download)

## Technologies implemented:

- .NET Core 3.1
- ASP.NET WebApi Core with JWT Bearer Authentication
- ASP.NET Identity Core
- Entity Framework Core 3.1
- .NET Core Native DI
- AutoMapper
- FluentValidator
- Swagger UI com JWT support
- Health Checks
- Redis
- Ip Rate Limit 
- OWASP Security
- Serilog
- Datasul / Seq

## Architecture:

- Full architecture with responsibility separation concerns, SOLID and Clean Code
- Domain Driven Design (Layers and Domain Model Pattern)
- Domain Events
- Domain Notification
- Unit of Work
- Repository and Generic Repository
