using Platform.Domain.Enums;
using Platform.Domain.Exceptions;

namespace Platform.Domain.Entities;


public class TaskItem
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public Priority Priority { get; private set; }
    public int Order { get; private set; }
    public bool IsCompleted { get; private set; }

    // Navigation property for EF Core
    public Project Project { get; private set; } = null!;

    // Required by EF Core
    private TaskItem() { }

    public static TaskItem Create(Guid projectId, string title, Priority priority, int order)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("El título de la tarea es obligatorio.");

        if (order < 1)
            throw new DomainException("El orden debe ser un número positivo.");

        return new TaskItem
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Title = title.Trim(),
            Priority = priority,
            Order = order,
            IsCompleted = false
        };
    }

    public void Update(string title, Priority priority, bool isCompleted)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("El título de la tarea es obligatorio.");

        Title = title.Trim();
        Priority = priority;
        IsCompleted = isCompleted;
    }

    public void SetCompletionStatus(bool isCompleted)
    {
        IsCompleted = isCompleted;
    }

    public void MarkAsCompleted()
    {
        IsCompleted = true;
    }

    public void ChangeOrder(int newOrder)
    {
        if (newOrder < 1)
            throw new DomainException("El orden debe ser un número positivo.");

        Order = newOrder;
    }
}
