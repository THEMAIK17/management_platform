using Platform.Domain.Enums;

namespace Platform.Application.DTOs;

public record ProjectDto(
    Guid Id,
    Guid UserId,
    string Name,
    string Description,
    ProjectStatus Status,
    int TaskCount,
    int CompletedTaskCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateProjectDto(
    string Name,
    string Description,
    Guid UserId
);

public record UpdateProjectDto(
    string Name,
    string Description
);

public record ProjectSummaryDto(
    int TotalProjects,
    int ActiveProjects,
    int CompletedProjects,
    int TotalTasks
);
