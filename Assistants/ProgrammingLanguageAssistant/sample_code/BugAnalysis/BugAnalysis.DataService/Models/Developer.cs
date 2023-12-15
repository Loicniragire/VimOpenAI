using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class Developer
{
    public string DeveloperKey { get; set; }

    public int YearsOfExpirience { get; set; }

    public virtual ICollection<DailyMeetingParticipant> DailyMeetingParticipants { get; set; } = new List<DailyMeetingParticipant>();

    public virtual ICollection<DeveloperFeature> DeveloperFeatures { get; set; } = new List<DeveloperFeature>();
}
