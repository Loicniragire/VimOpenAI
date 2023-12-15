using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class StakeHolderDepartment
{
    public int StakeHolderDepartmentId { get; set; }

    public string DepartmentName { get; set; }

    public virtual ICollection<StakeHolder> StakeHolders { get; set; } = new List<StakeHolder>();
}
