using Platform.Application.Common.Models;
using Platform.Application.DTOs;
using Platform.Application.Interfaces;
using Platform.Domain.Entities;
using Platform.Domain.Enums;
using Platform.Domain.Exceptions;
using Platform.Domain.Interfaces;

namespace Platform.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _uow;

    public ProjectService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ProjectDto> GetByIdAsync(Guid id)
    {
        var project = await _uow.Projects.GetByIdAsync(id);
        if (project == null) throw new DomainException("Proyecto no encontrado.");

        return MapToDto(project);
    }

    public async Task<PagedResult<ProjectDto>> GetPagedAsync(int page, int pageSize, string? statusFilter)
    {
        var (items, totalCount) = await _uow.Projects.GetPagedAsync(page, pageSize, statusFilter);
        var dtos = items.Select(MapToDto);

        return new PagedResult<ProjectDto>(dtos, totalCount, page, pageSize);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
    {
        var project = Project.Create(dto.Name, dto.Description);
        await _uow.Projects.AddAsync(project);
        await _uow.SaveChangesAsync();

        return MapToDto(project);
    }

    public async Task UpdateAsync(Guid id, UpdateProjectDto dto)
    {
        var project = await _uow.Projects.GetByIdAsync(id);
        if (project == null) throw new DomainException("Proyecto no encontrado.");

        project.Update(dto.Name, dto.Description);
        await _uow.Projects.UpdateAsync(project);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var project = await _uow.Projects.GetByIdAsync(id);
        if (project == null) throw new DomainException("Proyecto no encontrado.");

        await _uow.Projects.DeleteAsync(project);
        await _uow.SaveChangesAsync();
    }

    public async Task ActivateAsync(Guid id)
    {
        var project = await _uow.Projects.GetByIdAsync(id);
        if (project == null) throw new DomainException("Proyecto no encontrado.");

        var tasks = await _uow.Tasks.GetByProjectIdAsync(id);
        project.Activate(tasks.Count());

        await _uow.Projects.UpdateAsync(project);
        await _uow.SaveChangesAsync();
    }

    public async Task CompleteAsync(Guid id)
    {
        var project = await _uow.Projects.GetByIdAsync(id);
        if (project == null) throw new DomainException("Proyecto no encontrado.");

        var tasks = await _uow.Tasks.GetByProjectIdAsync(id);
        var totalTasks = tasks.Count();
        var completedTasks = tasks.Count(t => t.IsCompleted);

        project.Complete(totalTasks, completedTasks);

        await _uow.Projects.UpdateAsync(project);
        await _uow.SaveChangesAsync();
    }

    public async Task<ProjectSummaryDto> GetSummaryAsync()
    {
        // For a more performant summary, we'd add a specialized method to IProjectRepository
        // but for now we can use the existing paged method with page 1, size max
        var (allProjects, totalCount) = await _uow.Projects.GetPagedAsync(1, int.MaxValue, null);
        var projectsList = allProjects.ToList();

        var (allTasks, totalTasksCount) = (Enumerable.Empty<TaskItem>(), 0); // We don't have GetPaged for tasks yet
        // Let's just sum up from the projects loaded
        var totalTasks = projectsList.Sum(p => p.Tasks.Count);

        return new ProjectSummaryDto(
            TotalProjects: totalCount,
            ActiveProjects: projectsList.Count(p => p.Status == ProjectStatus.Active),
            CompletedProjects: projectsList.Count(p => p.Status == ProjectStatus.Completed),
            TotalTasks: totalTasks
        );
    }

    private static ProjectDto MapToDto(Project p)
    {
        return new ProjectDto(
            p.Id,
            p.Name,
            p.Description,
            p.Status,
            p.Tasks.Count,
            p.Tasks.Count(t => t.IsCompleted)
        );
    }
}
