using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tasks.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();
var tasks = app.MapGroup("/tasks");
tasks.MapGet("/", async (AppDbContext db) =>
    await db.TaskItems.ToListAsync());

tasks.MapGet("/{id}", async (int id, AppDbContext db) =>
    await db.TaskItems.FindAsync(id)
        is TaskItem task
            ? Results.Ok(task)
            : Results.NotFound());
tasks.MapPost("/", async (TaskItem task, AppDbContext db) =>
{
    db.TaskItems.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/tasks/{task.Id}", task);
});
tasks.MapPut("/{id}", async (int id, TaskItem inputTask, AppDbContext db) =>
{
    var task = await db.TaskItems.FindAsync(id);
    if (task is null) return Results.NotFound();
    task.Title = inputTask.Title;
    task.Description = inputTask.Description;
    task.IsCompleted = inputTask.IsCompleted;
    await db.SaveChangesAsync();
    return Results.NoContent();
});
tasks.MapDelete("/{id}", async (int id, AppDbContext db) =>
{
    var task = await db.TaskItems.FindAsync(id);
    if (task is null) return Results.NotFound();
    db.TaskItems.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
