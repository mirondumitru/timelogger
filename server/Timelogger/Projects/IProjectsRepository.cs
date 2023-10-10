using System.Collections.Generic;
using System.Threading.Tasks;
using Timelogger.Foundations.Queries;

namespace Timelogger.Projects;

public interface IProjectsRepository
{
    Task<IEnumerable<Project>> GetAsync(Sort sort);

    Task<Project> GetAsync(int id);
}