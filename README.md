Legacy preservation notice
==========================

> **Attention:** this repository preserves a legacy ASP.NET Core Web API originally built for .NET Core 3.1. .NET Core 3.1 reached end of support on December 13, 2022.

The active solution is being modernized incrementally on Phase 2 and now targets .NET 10. The legacy baseline remains preserved separately so modernization work can compare against the original behavior without changing the historical reference.

The preserved legacy version is identified by the tag `v1.0.0-legacy` and by the branch `legacy/netcoreapp3.1`.

See [LEGACY.md](LEGACY.md) for the documented legacy requirements, commands, migrations, seed status, limitations, and validation notes.

What is the Project?
=====================
The objective of this project was to implement the most commonly used technologies, and to share as a base project for WEB API. The original implementation was .NET Core 3.1; the active solution now targets .NET 10.

## Give a Star! :star:
If you liked the project or if project helped you, please give a star ;)

## How to use:
- You will need the latest Visual Studio 2019 and the latest .NET Core SDK.
- ***Please check if you have installed the same runtime version (SDK) described in global.json***
- The latest SDK and tools can be downloaded from https://dot.net/core.

Also you can run the Project in Visual Studio Code (Windows, Linux or MacOS).

To know more about how to setup your enviroment visit the [Microsoft .NET Download Guide](https://www.microsoft.com/net/download)

## Technologies implemented:

- .NET 10
- Preserved .NET Core 3.1 legacy baseline
- ASP.NET WebApi Core with JWT Bearer Authentication
- ASP.NET Identity Core
- Entity Framework Core 10
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
