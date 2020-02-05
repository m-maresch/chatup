# ChatUp
ChatUp is a **chat application** (for Android) developed by a few colleagues and me. It offers basic chat functionality and user management. A special feature is the ability to **chat people "up"**, basically connecting strangers with similar interests. The backend was developed as an **Microservices Architecture** and this respository contains the source code for (basically everything I developed) the Gateway Gatekeeper Service, Notification Service and User Service as well as the shared parts among the services (Common) and the Mock Server used for testing by the frontend team. The services use **resilience patterns, messaging, relational databases** (per service), **distributed caching** (per service) and are **highly configurable** (via environment variables). For details about the architecture and the responsibility of each service, read the blog post titled *A Microservices-based Chat Backend – System Design* on [mmaresch.com](http://mmaresch.com).

If you have any questions about the application or you'd like to know how to build, configure and deploy the 3 microservices in this repository then feel free to contact me via my website.

# Dependencies
Thanks to everyone contributing to any of the following projects:
- ASP.NET Core (and every project included in Microsoft.AspNetCore.All)
- Entity Framework Core
- SignalR
- AutoMapper
- Rx.NET
- Fluent Assertions
- xUnit.net
- Autofac
- FluentValidation
- IdentityServer4
- MassTransit
- Npgsql
- Polly
- Java-WebSocket
- OkHttp
- Gson
- JSON-Java (org.json)
