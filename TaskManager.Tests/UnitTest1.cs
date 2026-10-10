using Microsoft.EntityFrameworkCore;
using System;
using TaskManager.Api.Data;
using TaskManager.Api.Models;

namespace TaskManager.Tests;

public class TaskItemTests
{
    private static AppDbContext CreatedDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
    [Fact]
    public async Task AddTask_GeneratesId()
    {
        using var db = CreatedDb();
        db.TaskItems.Add(new TaskItem { Title = "Test" });
        await db.SaveChangesAsync();

        var task = await db.TaskItems.FirstAsync();
        Assert.True(task.Id > 0);
    }
    [Fact]
    public async Task NewTask_IsNotCompletedByDefault()
    {
        using var db = CreatedDb();
        db.TaskItems.Add(new TaskItem { Title = "Test" });
        await db.SaveChangesAsync();

        var task = await db.TaskItems.FirstAsync();
        Assert.False(task.IsCompleted);
    }
    [Fact]
    public async Task RemoveTask_DeletesIt()
    {
        using var db = CreatedDb();
        var task = new TaskItem { Title = "Test" };
        db.TaskItems.Add(task);
        await db.SaveChangesAsync();

        db.TaskItems.Remove(task);
        await db.SaveChangesAsync();

        Assert.Empty(await db.TaskItems.ToListAsync()); 
    }
}