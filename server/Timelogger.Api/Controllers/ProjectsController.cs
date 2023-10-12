using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Timelogger.Foundations.Queries;
using Timelogger.Projects;

namespace Timelogger.Api.Controllers;

[Route("api/[controller]")]
public class ProjectsController : BaseController
{
    private readonly IProjectsRepository _projectsRepository;

    public ProjectsController(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Sort sort)
    {
        var projects = await _projectsRepository.GetAsync(sort);

        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var projects = await _projectsRepository.GetAsync(id);

        return Ok(projects);
    }
}