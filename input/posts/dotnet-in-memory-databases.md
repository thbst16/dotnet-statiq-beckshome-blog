Title: Dotnet In Memory Databases
Published: 10/9/2022
Tags:
    - .NET
    - Technology Guides
---
My popular [dotnet-blazor-crud](https://github.com/thbst16/dotnet-blazor-crud) project has long used the [EF Core InMemory database provider](https://exceptionnotfound.net/ef-core-inmemory-asp-net-core-store-database/) for data persistence. While this hasn't caused me any issues -- ever, I've been aspiring to move to a relational database versus an in-memory provider. SQLite is the obvious contender here, without going to a full out-of-process database. Getting started with SQLite and EF Core is [pretty easy](https://www.koderdojo.com/blog/getting-started-with-entity-framework-core-and-sqlite). The real question for me was how much work it would take to swap out the in memory provider for SQLite.

Since SQLite is a file-based database and BlazorCrud is a Docker-based solution, I had to decide whether or not to [persist the DB](https://docs.docker.com/get-started/05_persisting_data/) outside of the container. I use container volumes in many of my other projects but, since one of my goals of BlazorCrud is to have a pure Docker distribution (that is, no depdencies on mounts or Docker Compose), long term persistence was out.

With that decision out of the way, the steps to swap the EF Core InMemory provider for SQLite (or vice-versa) are pretty simple:

1. <b>Update DB Package References.</b> This is a simple swap of the Microsoft.EntityFrameworkCore.Sqlite provider for the Microsoft.EntityFrameworkCore.InMemory provider. Alternately, you could leave both there and switch between in memory and SQLite using configuration switches.
2. <b>Update AppDbContext Options.</b> This simply involves swapping in the UseSqlLite method (with a pointer to a physical file) for the UseInMemoryDatabase method.