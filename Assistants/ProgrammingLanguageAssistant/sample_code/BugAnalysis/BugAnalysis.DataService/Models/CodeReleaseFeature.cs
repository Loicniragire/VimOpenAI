using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class CodeReleaseFeature
{
    public int CodeReleaseFeatureId { get; set; }

    public string CodeReleaseKey { get; set; }

    public string CodeFeatureKey { get; set; }

    public virtual CodeFeature CodeFeatureKeyNavigation { get; set; }

    public virtual CodeRelease CodeReleaseKeyNavigation { get; set; }
}
