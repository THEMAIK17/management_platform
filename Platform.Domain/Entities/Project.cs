using Platform.Domain.Enums;
using Platform.Domain.Exceptions;

namespace Platform.Domain.Entities;

public class Project
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public ProjectStatus Status { get; private set; }

    // Private setter prevents external reassignment; EF Core can still populate the collection.
    public ICollection<TaskItem> Tasks { get; private set; } = new List<TaskItem>();

    // Required by EF Core for materialisation — not intended for application use.
    private Project() { }

    public static Project Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre del proyecto es obligatorio.");

        return new Project
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim() ?? string.Empty,
            Status = ProjectStatus.Draft
        };
    }

    public void Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre del proyecto es obligatorio.");

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
    }

    // Business Rule: a project must have at least one task to be activated.
    public void Activate(int taskCount)
    {
        if (taskCount == 0)
            throw new DomainException("El proyecto debe tener al menos una tarea para poder activarse.");

        if (Status == ProjectStatus.Completed)
            throw new DomainException("Un proyecto completado no puede reactivarse.");

        Status = ProjectStatus.Active;
    }

    // Business Rule: all tasks must be completed before the project can be closed.
    public void Complete(int totalTasks, int completedTasks)
    {
        if (Status != ProjectStatus.Active)
            throw new DomainException("Solo un proyecto activo puede completarse.");

        if (totalTasks == 0 || completedTasks < totalTasks)
            throw new DomainException("Todas las tareas deben estar completadas para cerrar el proyecto.");

        Status = ProjectStatus.Completed;
    }
}
