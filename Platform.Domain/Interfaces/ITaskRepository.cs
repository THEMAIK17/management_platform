using Platform.Domain.Entities;

namespace Platform.Domain.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId);
    Task AddAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
    Task<int> GetMaxOrderAsync(Guid projectId);
    Task<bool> IsOrderUniqueAsync(Guid projectId, int order, Guid? excludeTaskId = null);
    Task<IEnumerable<TaskItem>> GetTasksToReorderAsync(Guid projectId, int fromOrder, int toOrder);
}
