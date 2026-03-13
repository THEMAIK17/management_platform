using Platform.Application.DTOs;

namespace Platform.Application.Interfaces;

public interface ITaskService
{
    Task<TaskItemDto> GetByIdAsync(Guid id);
    Task<IEnumerable<TaskItemDto>> GetByProjectIdAsync(Guid projectId);
    Task<TaskItemDto> CreateAsync(Guid projectId, CreateTaskDto dto);
    Task UpdateAsync(Guid id, UpdateTaskDto dto);
    Task DeleteAsync(Guid id);
    Task CompleteAsync(Guid id);
    Task ReorderAsync(Guid id, int newOrder);
}
