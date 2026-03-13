using Platform.Domain.Enums;

namespace Platform.Application.DTOs;

public record TaskItemDto(
    Guid Id,
    Guid ProjectId,
    string Title,
    Priority Priority,
    int Order,
    bool IsCompleted
);

public record CreateTaskDto(
    string Title,
    Priority Priority,
    int Order
);

public record UpdateTaskDto(
    string Title,
    Priority Priority,
    int Order,
    bool IsCompleted
);

public record ReorderTaskDto(
    int NewOrder
);
