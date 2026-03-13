using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Application.DTOs;
using Platform.Application.Interfaces;
using Platform.Domain.Exceptions;

namespace Platform.Api.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    // POST /api/tasks/{projectId}
    [HttpPost("{projectId:guid}")]
    public async Task<IActionResult> Create(Guid projectId, [FromBody] CreateTaskDto dto)
    {
        try
        {
            var task = await _taskService.CreateAsync(projectId, dto);
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET /api/tasks/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var task = await _taskService.GetByIdAsync(id);
            return Ok(task);
        }
        catch (DomainException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // PUT /api/tasks/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto)
    {
        try
        {
            await _taskService.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE /api/tasks/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _taskService.DeleteAsync(id);
            return NoContent();
        }
        catch (DomainException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // PATCH /api/tasks/{id}/complete
    [HttpPatch("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        try
        {
            await _taskService.CompleteAsync(id);
            return Ok(new { message = "Tarea marcada como completada." });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // PATCH /api/tasks/{id}/reorder
    [HttpPatch("{id:guid}/reorder")]
    public async Task<IActionResult> Reorder(Guid id, [FromBody] ReorderTaskDto dto)
    {
        try
        {
            await _taskService.ReorderAsync(id, dto.NewOrder);
            return Ok(new { message = "Tarea reordenada correctamente." });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
