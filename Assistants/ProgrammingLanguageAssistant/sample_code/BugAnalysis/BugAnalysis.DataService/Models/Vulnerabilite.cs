using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class Vulnerabilite
{
    public int VulnetabilityId { get; set; }

    public string Framework { get; set; }

    public string FrameworkVersion { get; set; }

    public string LibraryName { get; set; }

    public string LibraryVersion { get; set; }

    public string VulnerabiltyName { get; set; }

    public string VulnerabilityDescription { get; set; }

    public string ResolvingVersion { get; set; }
}
