using System.Collections.Generic;
using System.Threading.Tasks;

namespace Timelogger.TimeRegistrations;

public interface ITimeRegistrationsRepository
{
    Task<IEnumerable<TimeRegistration>> Get(int? projectId);

    Task<TimeRegistration> SaveAsync(TimeRegistration timeRegistration);
}