using Platform.Application.Common.Models;
using Platform.Application.DTOs;

namespace Platform.Application.Interfaces;

public interface IProjectService
{
    Task<ProjectDto> GetByIdAsync(Guid id);
    Task<PagedResult<ProjectDto>> GetPagedAsync(int page, int pageSize, string? statusFilter);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto);
    Task UpdateAsync(Guid id, UpdateProjectDto dto);
    Task DeleteAsync(Guid id);
    Task ActivateAsync(Guid id);
    Task CompleteAsync(Guid id);
    Task<ProjectSummaryDto> GetSummaryAsync();
}
