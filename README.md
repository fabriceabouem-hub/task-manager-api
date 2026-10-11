# Task Manager API

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

```powershell
Invoke-RestMethod -Method Post -Uri http://localhost:5032/tasks `
  -ContentType "application/json" `
  -Body '{"title":"My first task","description":"Hello API"}'
```

## Run the tests

```bash
cd TaskManager.Tests
dotnet test
```

## Roadmap
- [ ] CI with GitHub Actions
- [ ] Docker support
- [ ] JWT authentication
