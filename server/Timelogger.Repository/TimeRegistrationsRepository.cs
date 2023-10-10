using Microsoft.EntityFrameworkCore;
using Timelogger.TimeRegistrations;

namespace Timelogger.Repository;

public class TimeRegistrationsRepository : ITimeRegistrationsRepository
{
    private readonly ApiContext _context;

    public TimeRegistrationsRepository(ApiContext context)
    {
        _context = context;
    }

    public Task<IEnumerable<TimeRegistration>> Get(int? projectId)
    {
        IEnumerable<TimeRegistration> timeRegistrations = _context.TimeRegistrations
            .Include(x => x.Project);

        if (projectId.HasValue)
            timeRegistrations = timeRegistrations.Where(x => x.Project.Id == projectId);

        return Task.FromResult(timeRegistrations);
    }

    public Task<TimeRegistration> SaveAsync(TimeRegistration timeRegistration)
    {
        // manually add auto-increment ID - needs attention when switching to SQL DB 
        timeRegistration.Id = _context.TimeRegistrations.ToList().DefaultIfEmpty().Max(x => x?.Id ?? 0) + 1;

        _context.TimeRegistrations.Add(timeRegistration);
        _context.SaveChanges();

        return Task.FromResult(timeRegistration);
    }
}