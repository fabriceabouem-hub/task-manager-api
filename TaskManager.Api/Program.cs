using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Api.Data;
using TaskManager.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tasks.db"));

var jwtKey = builder.Configuration["Jwt:Key"]
   ?? throw new InvalidOperationException("Jwt:Key is not configured.");

// Configure JWT authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", (LoginRequest request, IConfiguration config) =>
{
    if (request.Username != config["Demo:Username"] || request.Password != config["Demo:Password"]) 
        return Results.Unauthorized();
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
    var token = new JwtSecurityToken (
        claims: new[]
        {
            new Claim(ClaimTypes.Name, request.Username)
        },expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

    return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
});
var tasks = app.MapGroup("/tasks").RequireAuthorization();

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
record LoginRequest(string Username, string Password);  
