﻿using Microsoft.EntityFrameworkCore;
using Timelogger.Projects;
using Timelogger.TimeRegistrations;

namespace Timelogger;

public class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<TimeRegistration> TimeRegistrations { get; set; }
}