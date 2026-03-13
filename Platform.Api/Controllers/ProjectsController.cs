using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Application.DTOs;
using Platform.Application.Interfaces;
using Platform.Domain.Exceptions;

namespace Platform.Api.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    // GET /api/projects/search?status=Active&page=1&pageSize=10
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _projectService.GetPagedAsync(page, pageSize, status);
        return Ok(result);
    }

    // GET /api/projects/{id}/tasks
    [HttpGet("{id:guid}/tasks")]
    public async Task<IActionResult> GetTasks(Guid id, [FromServices] ITaskService taskService)
    {
        try
        {
            var tasks = await taskService.GetByProjectIdAsync(id);
            return Ok(tasks);
        }
        catch (DomainException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // GET /api/projects/{id}/summary
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _projectService.GetSummaryAsync();
        return Ok(summary);
    }

    // POST /api/projects
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
    {
        try
        {
            var project = await _projectService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetTasks), new { id = project.Id }, project);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // PUT /api/projects/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectDto dto)
    {
        try
        {
            await _projectService.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (DomainException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // DELETE /api/projects/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _projectService.DeleteAsync(id);
            return NoContent();
        }
        catch (DomainException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // PATCH /api/projects/{id}/activate
    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        try
        {
            await _projectService.ActivateAsync(id);
            return Ok(new { message = "Proyecto activado correctamente." });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // PATCH /api/projects/{id}/complete
    [HttpPatch("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        try
        {
            await _projectService.CompleteAsync(id);
            return Ok(new { message = "Proyecto completado correctamente." });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
