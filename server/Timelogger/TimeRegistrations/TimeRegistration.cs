﻿using System;
using Timelogger.Projects;

namespace Timelogger.TimeRegistrations;

public class TimeRegistration
{
    public int Id { get; set; }
    public Project Project { get; set; }
    public DateTime ValueDate { get; set; }
    public decimal Hours { get; set; }
    public DateTime? CreatedAtUtc { get; set; }
}