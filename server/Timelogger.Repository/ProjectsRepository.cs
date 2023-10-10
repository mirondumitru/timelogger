using Microsoft.EntityFrameworkCore;
using Timelogger.Foundations.Queries;
using Timelogger.Projects;

namespace Timelogger.Repository;

public class ProjectsRepository : IProjectsRepository
{
    private readonly ApiContext _context;

    public ProjectsRepository(ApiContext context)
    {
        _context = context;
    }

    public Task<IEnumerable<Project>> GetAsync(Sort sort)
    {
        var projects = _context.Projects;

        var sortedProjects = Sort(projects, sort);

        return Task.FromResult(sortedProjects);
    }

    public Task<Project?> GetAsync(int id)
    {
        var project = _context.Projects.SingleOrDefault(x => x.Id == id);

        return Task.FromResult(project);
    }

    private static IEnumerable<Project> Sort(DbSet<Project> projects, Sort sort)
    {
        var query = projects.AsQueryable();

        return sort.SortBy.ToUpperInvariant() switch
        {
            "Deadline" => sort.SortOrder.ToUpperInvariant() == "DESC"
                ? query.OrderBy(x => x.Deadline)
                : query.OrderByDescending(x => x.Deadline),

            _ => sort.SortOrder.ToUpperInvariant() == "DESC"
                ? query.OrderBy(x => x.Id)
                : query.OrderByDescending(x => x.Id)
        };
    }
}