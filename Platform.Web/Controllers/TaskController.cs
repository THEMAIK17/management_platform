using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Application.DTOs;
using Platform.Application.Interfaces;
using Platform.Domain.Exceptions;

namespace Platform.Web.Controllers;

[Authorize]
public class TaskController : Controller
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    // GET /Task/Create?projectId={id}
    public IActionResult Create(Guid projectId)
    {
        ViewBag.ProjectId = projectId;
        return View();
    }

    // POST /Task/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid projectId, CreateTaskDto dto)
    {
        try
        {
            await _taskService.CreateAsync(projectId, dto);
            return RedirectToAction("Tasks", "Project", new { id = projectId });
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError("", ex.Message);
            ViewBag.ProjectId = projectId;
            return View(dto);
        }
    }

    // GET /Task/Edit/{id}
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var task = await _taskService.GetByIdAsync(id);
            return View(new UpdateTaskDto(task.Title, task.Priority, task.Order, task.IsCompleted));
        }
        catch (DomainException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Project");
        }
    }

    // POST /Task/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateTaskDto dto, Guid projectId)
    {
        try
        {
            await _taskService.UpdateAsync(id, dto);
            return RedirectToAction("Tasks", "Project", new { id = projectId });
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }
    }

    // POST /Task/Delete/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid projectId)
    {
        try
        {
            await _taskService.DeleteAsync(id);
        }
        catch (DomainException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Tasks", "Project", new { id = projectId });
    }

    // POST /Task/Complete/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(Guid id, Guid projectId)
    {
        try
        {
            await _taskService.CompleteAsync(id);
            TempData["Success"] = "Tarea completada.";
        }
        catch (DomainException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Tasks", "Project", new { id = projectId });
    }

    // POST /Task/Reorder/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reorder(Guid id, int newOrder, Guid projectId)
    {
        try
        {
            await _taskService.ReorderAsync(id, newOrder);
        }
        catch (DomainException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Tasks", "Project", new { id = projectId });
    }
}
