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

        // Returns 0 when no tasks exist, allowing callers to start at order 1.
        return tasks.Any() ? tasks.Max(t => t.Order) : 0;
    }

    public async Task<bool> IsOrderUniqueAsync(Guid projectId, int order, Guid? excludeTaskId = null)
    {
        var query = _context.Tasks
            .Where(t => t.ProjectId == projectId && t.Order == order);

        // Exclude the task being updated so it doesn't conflict with its own current order.
        if (excludeTaskId.HasValue)
            query = query.Where(t => t.Id != excludeTaskId.Value);

        return !await query.AnyAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetTasksToReorderAsync(Guid projectId, int fromOrder, int toOrder)
    {
        // Fetch all tasks in the affected range to shift them up or down during reorder.
        var min = Math.Min(fromOrder, toOrder);
        var max = Math.Max(fromOrder, toOrder);

        return await _context.Tasks
            .Where(t => t.ProjectId == projectId && t.Order >= min && t.Order <= max)
            .OrderBy(t => t.Order)
            .ToListAsync();
    }
}
