using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class Bug
{
    public string BugKey { get; set; }

    public DateTime ReportedDatetimeUtc { get; set; }

    public string Description { get; set; }

    public int SeverityLevelId { get; set; }

    public virtual SeverityLevel SeverityLevel { get; set; }
}
