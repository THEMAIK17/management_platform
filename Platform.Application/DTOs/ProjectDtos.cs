using Platform.Domain.Enums;

namespace Platform.Application.DTOs;

public record ProjectDto(
    Guid Id,
    string Name,
    string Description,
    ProjectStatus Status,
    int TaskCount,
    int CompletedTaskCount
);

public record CreateProjectDto(
    string Name,
    string Description
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
