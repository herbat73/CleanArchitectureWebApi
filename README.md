[![Build](https://github.com/herbat73/CleanArchitectureWebApi/actions/workflows/build_and_test.yml/badge.svg)](https://github.com/herbat73/CleanArchitectureWebApi/actions/workflows/build_and_test.yml)

# Clean Architecture WebApi, No FrontEnd

Clean Architecture starter for .NET 10
It is simplified and modified version based on concept taken from [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/CleanArchitecture) 

Differences:
- No MediatR (WolverineFx used)
- No AutoMapper (refactored code)
- No Aspire stack
- No NUnit (XUnit used)
- Functional tests are using TestContainers in order to run on GitHub test actions
- Fixes & refactoring

## What's included in the template?

- **Shared** project with common Domain-Driven Design abstractions.
- **Domain** layer with sample entities and domain events.
- **Application** layer with abstractions for:
    - CQRS (lightweight, WolverineFx command/query handlers)
    - Example use cases (Todos,WeatherForecasts and Users)
    - Cross-cutting concerns (logging, validation) implemented as decorators
- **Infrastructure** layer with:
    - JWT authentication with **refresh tokens** (with token rotation)
    - Permission-based authorization
    - EF Core + PostgreSQL
- **Web.Api** layer with:
    - Minimal API endpoints
    - **OpenTelemetry** tracing and metrics (ASP.NET Core, HTTP, Npgsql, runtime)
    - Global exception handling and `ProblemDetails`
    - Scalar API reference (OpenAPI) with JWT support
- **Testing** projects
    - Functional testing (`Application.FunctionalTests`)
    - Unit testing (`Application.UnitTests`)
    - Domain testing (`Domain.UnitTests`)

## Getting started

## Run docker and create container for local development with Postgres 

Run `docker-compose up -d` to pull and start postgress db

## Build

Run `dotnet build` to build the solution.

## Run

To run the application:

```bash
dotnet run --project .\src\Web
```

The application expects PostgreSQL on `127.0.0.1:5432`, configured in `src\Web\appsettings.json`. The API reference is served at `/scalar`.

## Client app (Angular)

The Angular client lives in `src/ClientApp` and calls the WebApi through `/api`. In development, `ng serve` forwards `/api` to the WebApi (`proxy.conf.json`), so no CORS setup is needed for the client.

Both services must run:

1. WebApi on `http://localhost:5117` (Terminal 1):
   ```bash
   dotnet run --project .\src\Web --launch-profile http
   ```
2. Angular client on `http://localhost:4200` (Terminal 2):
   ```bash
   cd src\ClientApp
   npm install
   npm start
   ```

Open `http://localhost:4200` and sign in. The development database is seeded with `administrator@localhost` / `Administrator1!`. The client stores the bearer tokens in `localStorage` and refreshes them on a 401.

The WebApi can also be called directly, for example from the Scalar reference at `http://localhost:5117/scalar` or from `Web.http`.

Client build and tests:
```bash
cd src\ClientApp
npm run build
npm test -- --watch=false
```

## Test

The solution contains unit, integration, and functional tests.

To run the tests:
```bash
dotnet test
```

The functional tests start their own PostgreSQL container with [Testcontainers](https://testcontainers.com/), so Docker must be running. The container is created once per test run, and tables are reset between tests. No local PostgreSQL server is needed.
