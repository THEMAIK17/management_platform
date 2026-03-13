using Microsoft.AspNetCore.Mvc;
using Platform.Application.DTOs;
using Platform.Application.Interfaces;
using Platform.Domain.Exceptions;

namespace Platform.Web.Controllers;

public class ProjectController : Controller
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    // GET /Project
    public async Task<IActionResult> Index(string? status, int page = 1)
    {
        var result = await _projectService.GetPagedAsync(page, 10, status);
        ViewBag.StatusFilter = status;
        return View(result);
    }

    // GET /Project/Create
    public IActionResult Create() => View();

    // POST /Project/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProjectDto dto)
    {
        try
        {
            await _projectService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }
    }

    // GET /Project/Edit/{id}
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var project = await _projectService.GetByIdAsync(id);
            return View(new UpdateProjectDto(project.Name, project.Description));
        }
        catch (DomainException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    // POST /Project/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateProjectDto dto)
    {
        try
        {
            await _projectService.UpdateAsync(id, dto);
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }
    }

    // POST /Project/Delete/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _projectService.DeleteAsync(id);
        }
        catch (DomainException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    // POST /Project/Activate/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(Guid id)
    {
        try
        {
            await _projectService.ActivateAsync(id);
            TempData["Success"] = "Proyecto activado correctamente.";
        }
        catch (DomainException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    // POST /Project/Complete/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(Guid id)
    {
        try
        {
            await _projectService.CompleteAsync(id);
            TempData["Success"] = "Proyecto completado.";
        }
        catch (DomainException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    // GET /Project/Tasks/{id}  
    public async Task<IActionResult> Tasks(Guid id, [FromServices] ITaskService taskService)
    {
        try
        {
            var project = await _projectService.GetByIdAsync(id);
            var tasks = await taskService.GetByProjectIdAsync(id);
            ViewBag.Project = project;
            return View(tasks);
        }
        catch (DomainException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
