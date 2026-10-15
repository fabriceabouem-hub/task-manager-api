# Task Manager API
[![.NET Tests](https://github.com/fabriceabouem-hub/Task-Manager-API/actions/workflows/dotnet.yml/badge.svg)](https://github.com/fabriceabouem-hub/Task-Manager-API/actions/workflows/dotnet.yml)

A REST API for managing tasks, built with ASP.NET Core 8 and Entity Framework Core.
Personal project to practice backend development.

## Tech Stack
- C# / ASP.NET Core 8 (Minimal API)
- Entity Framework Core with SQLite
- xUnit (unit tests)

## Endpoints

| Method | Route         | Description        |
|--------|---------------|--------------------|
| GET    | /tasks        | List all tasks     |
| GET    | /tasks/{id}   | Get a task by id   |
| POST   | /tasks        | Create a task      |
| PUT    | /tasks/{id}   | Update a task      |
| DELETE | /tasks/{id}   | Delete a task      |

## Getting Started

Requirements: .NET 8 SDK

```bash
git clone https://github.com/fabriceabouem-hub/Task-Manager-API.git
cd Task-Manager-API/TaskManager.Api
dotnet tool install --global dotnet-ef --version 8.0.11
dotnet ef database update
dotnet run
```

The API runs on `http://localhost:5032`.

### Example request
This request requires a token (see Authentification below).
```powershell
Invoke-RestMethod -Method Post -Uri http://localhost:5032/tasks `
  -ContentType "application/json" `
  -Body '{"title":"My first task","description":"Hello API"}'
```
## Authentication

The `/tasks` endpoints require a JWT token. Log in via `POST /login`.
Credentials and signing key are read from configuration (user secrets in development):

```bash
cd TaskManager.Api
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "a-long-secret-key-of-at-least-32-characters"
dotnet user-secrets set "Demo:Username" "admin"
dotnet user-secrets set "Demo:Password" "your-password"
```

```powershell
$r = Invoke-RestMethod -Method Post -Uri http://localhost:5032/login `
  -ContentType "application/json" `
  -Body '{"username":"admin","password":"your-password"}'
Invoke-RestMethod -Uri http://localhost:5032/tasks `
  -Headers @{ Authorization = "Bearer $($r.token)" }
```
## Run the tests

```bash
cd TaskManager.Tests
dotnet test
```

## Roadmap
- [X] CI with GitHub Actions
- [X] Docker support
- [ ] JWT authentication
