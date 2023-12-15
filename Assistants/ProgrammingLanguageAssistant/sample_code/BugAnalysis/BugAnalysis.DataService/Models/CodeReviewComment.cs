using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class CodeReviewComment
{
    public int ReviewCommentId { get; set; }

    public string CodeReviewKey { get; set; }

    public int CommentCategoryId { get; set; }

    public int CodeBaseFileId { get; set; }

    public int StartingFromLineNumber { get; set; }

    public int EndingAtLineNumber { get; set; }

    public string ReviewComment { get; set; }

    public virtual CodeBaseFile CodeBaseFile { get; set; }

    public virtual CodeReviewCommentCategory CommentCategory { get; set; }
}
