using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class CodeBaseLibrary
{
    public int CodeBaseLibraryId { get; set; }

    public string CodeBaseKey { get; set; }

    public string LibraryName { get; set; }

    public string LibraryVersion { get; set; }

    public virtual CodeBase CodeBaseKeyNavigation { get; set; }
}
