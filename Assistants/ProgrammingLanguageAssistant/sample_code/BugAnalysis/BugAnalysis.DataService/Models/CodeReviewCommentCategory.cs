using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class CodeReviewCommentCategory
{
    public int CommentCategoryId { get; set; }

    public string CommentCategory { get; set; }

    public string CategoryDescription { get; set; }

    public virtual ICollection<CodeReviewComment> CodeReviewComments { get; set; } = new List<CodeReviewComment>();
}
