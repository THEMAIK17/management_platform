using Microsoft.EntityFrameworkCore;
using Platform.Domain.Entities;
using Platform.Domain.Interfaces;
using Platform.Infrastructure.Data;

namespace Platform.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Order)
            .ToListAsync();
    }

    public async Task AddAsync(TaskItem task)
    {
        await _context.Tasks.AddAsync(task);
    }

    public Task UpdateAsync(TaskItem task)
    {
        _context.Tasks.Update(task);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TaskItem task)
    {
        _context.Tasks.Remove(task);
        return Task.CompletedTask;
    }

    public async Task<int> GetMaxOrderAsync(Guid projectId)
    {
        var tasks = await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .ToListAsync();

        return tasks.Any() ? tasks.Max(t => t.Order) : 0;
    }

    public async Task<bool> IsOrderUniqueAsync(Guid projectId, int order, Guid? excludeTaskId = null)
    {
        var query = _context.Tasks
            .Where(t => t.ProjectId == projectId && t.Order == order);

        if (excludeTaskId.HasValue)
            query = query.Where(t => t.Id != excludeTaskId.Value);

        return !await query.AnyAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetTasksToReorderAsync(Guid projectId, int fromOrder, int toOrder)
    {
        var min = Math.Min(fromOrder, toOrder);
        var max = Math.Max(fromOrder, toOrder);

        return await _context.Tasks
            .Where(t => t.ProjectId == projectId && t.Order >= min && t.Order <= max)
            .OrderBy(t => t.Order)
            .ToListAsync();
    }
}
