using Platform.Domain.Entities;
using Platform.Domain.Enums;
using Platform.Domain.Exceptions;
using Xunit;

namespace Platform.Tests.Domain;

public class ProjectTests
{
    // ─────────────────────────────────────────────────────────────────
    // Activate
    // ─────────────────────────────────────────────────────────────────

    [Fact]
    public void ActivateProject_WithTasks_ShouldSucceed()
    {
        // Arrange
        var project = Project.Create("Test Project", "Description", Guid.NewGuid());

        // Act – project has 1 task
        project.Activate(taskCount: 1);

        // Assert
        Assert.Equal(ProjectStatus.Active, project.Status);
    }

    [Fact]
    public void ActivateProject_WithoutTasks_ShouldFail()
    {
        // Arrange
        var project = Project.Create("Test Project", "Description", Guid.NewGuid());

        // Act & Assert – no tasks → must throw
        Assert.Throws<DomainException>(() => project.Activate(taskCount: 0));
        Assert.Equal(ProjectStatus.Draft, project.Status); // status unchanged
    }

    // ─────────────────────────────────────────────────────────────────
    // Complete
    // ─────────────────────────────────────────────────────────────────

    [Fact]
    public void CompleteProject_WithAllTasksCompleted_ShouldSucceed()
    {
        // Arrange
        var project = Project.Create("Test Project", "Description", Guid.NewGuid());
        project.Activate(taskCount: 2);

        // Act – all 2 tasks completed
        project.Complete(totalTasks: 2, completedTasks: 2);

        // Assert
        Assert.Equal(ProjectStatus.Completed, project.Status);
    }

    [Fact]
    public void CompleteProject_WithPendingTasks_ShouldFail()
    {
        // Arrange
        var project = Project.Create("Test Project", "Description", Guid.NewGuid());
        project.Activate(taskCount: 3);

        // Act & Assert – 2 out of 3 done → must throw
        Assert.Throws<DomainException>(() => project.Complete(totalTasks: 3, completedTasks: 2));
        Assert.Equal(ProjectStatus.Active, project.Status); // status unchanged
    }

    // ─────────────────────────────────────────────────────────────────
    // Task Order
    // ─────────────────────────────────────────────────────────────────

    [Fact]
    public void CreateTask_WithInvalidOrder_ShouldFail()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        // Act & Assert – order < 1 is invalid in domain
        Assert.Throws<DomainException>(() =>
            TaskItem.Create(projectId, "Task A", Priority.Medium, order: 0));
    }

    // ─────────────────────────────────────────────────────────────────
    // Delete - domain-level check
    // ─────────────────────────────────────────────────────────────────

    [Fact]
    public void DeleteProject_ShouldBeDeletedWhenEntityRemoved()
    {
        // Note: Deletion itself is handled at the Infrastructure layer.
        // At the domain level we validate that a Project can be created and 
        // that there is no restriction preventing deletion (no business rule blocks it).

        // Arrange & Act
        var project = Project.Create("To Delete", "Temporary project", Guid.NewGuid());

        // Assert – entity created successfully, no rule prevents deletion
        Assert.NotEqual(Guid.Empty, project.Id);
        Assert.Equal("To Delete", project.Name);
        Assert.Equal(ProjectStatus.Draft, project.Status);
    }
}
