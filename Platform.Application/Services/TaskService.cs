using Platform.Application.DTOs;
using Platform.Application.Interfaces;
using Platform.Domain.Entities;
using Platform.Domain.Exceptions;
using Platform.Domain.Interfaces;

namespace Platform.Application.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _uow;

    public TaskService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<TaskItemDto> GetByIdAsync(Guid id)
    {
        var task = await _uow.Tasks.GetByIdAsync(id);
        if (task == null) throw new DomainException("Tarea no encontrada.");

        return MapToDto(task);
    }

    public async Task<IEnumerable<TaskItemDto>> GetByProjectIdAsync(Guid projectId)
    {
        var tasks = await _uow.Tasks.GetByProjectIdAsync(projectId);
        return tasks.Select(MapToDto);
    }

    public async Task<TaskItemDto> CreateAsync(Guid projectId, CreateTaskDto dto)
    {
        if (!await _uow.Projects.ExistsAsync(projectId))
            throw new DomainException("El proyecto no existe.");

        // Rule: Order must be unique within a project
        if (!await _uow.Tasks.IsOrderUniqueAsync(projectId, dto.Order))
            throw new DomainException($"Ya existe una tarea con el orden {dto.Order} en este proyecto.");

        var task = TaskItem.Create(projectId, dto.Title, dto.Priority, dto.Order);
        
        await _uow.Tasks.AddAsync(task);
        await _uow.SaveChangesAsync();

        return MapToDto(task);
    }

    public async Task UpdateAsync(Guid id, UpdateTaskDto dto)
    {
        var task = await _uow.Tasks.GetByIdAsync(id);
        if (task == null) throw new DomainException("Tarea no encontrada.");

        // Rule: If order changed, check uniqueness
        if (task.Order != dto.Order)
        {
            if (!await _uow.Tasks.IsOrderUniqueAsync(task.ProjectId, dto.Order, id))
                throw new DomainException($"Ya existe una tarea con el orden {dto.Order} en este proyecto.");
            
            task.ChangeOrder(dto.Order);
        }

        task.Update(dto.Title, dto.Priority, dto.IsCompleted);

        await _uow.Tasks.UpdateAsync(task);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var task = await _uow.Tasks.GetByIdAsync(id);
        if (task == null) throw new DomainException("Tarea no encontrada.");

        await _uow.Tasks.DeleteAsync(task);
        await _uow.SaveChangesAsync();
    }

    public async Task CompleteAsync(Guid id)
    {
        var task = await _uow.Tasks.GetByIdAsync(id);
        if (task == null) throw new DomainException("Tarea no encontrada.");

        task.MarkAsCompleted();

        await _uow.Tasks.UpdateAsync(task);
        await _uow.SaveChangesAsync();
    }

    public async Task ReorderAsync(Guid id, int newOrder)
    {
        var task = await _uow.Tasks.GetByIdAsync(id);
        if (task == null) throw new DomainException("Tarea no encontrada.");

        if (task.Order == newOrder) return;

        var currentOrder = task.Order;
        var projectId = task.ProjectId;

        // Get tasks between current and new order to shift them
        var tasksToShift = await _uow.Tasks.GetTasksToReorderAsync(projectId, currentOrder, newOrder);

        foreach (var t in tasksToShift)
        {
            if (t.Id == id)
            {
                t.ChangeOrder(newOrder);
            }
            else
            {
                // If moving down (current < new), shift intermediate tasks up (order--)
                // If moving up (current > new), shift intermediate tasks down (order++)
                int shift = (currentOrder < newOrder) ? -1 : 1;
                t.ChangeOrder(t.Order + shift);
            }
            await _uow.Tasks.UpdateAsync(t);
        }

        await _uow.SaveChangesAsync();
    }

    private static TaskItemDto MapToDto(TaskItem t)
    {
        return new TaskItemDto(
            t.Id,
            t.ProjectId,
            t.Title,
            t.Priority,
            t.Order,
            t.IsCompleted
        );
    }
}
