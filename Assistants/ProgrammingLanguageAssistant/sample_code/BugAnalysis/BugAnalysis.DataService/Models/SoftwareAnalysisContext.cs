using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BugAnalysis.DataService.Models;

public partial class SoftwareAnalysisContext : DbContext
{
    public SoftwareAnalysisContext()
    {
    }

    public SoftwareAnalysisContext(DbContextOptions<SoftwareAnalysisContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bug> Bugs { get; set; }

    public virtual DbSet<CodeBase> CodeBases { get; set; }

    public virtual DbSet<CodeBaseFile> CodeBaseFiles { get; set; }

    public virtual DbSet<CodeBaseLibrary> CodeBaseLibraries { get; set; }

    public virtual DbSet<CodeCommit> CodeCommits { get; set; }

    public virtual DbSet<CodeCommitFile> CodeCommitFiles { get; set; }

    public virtual DbSet<CodeCommitFileAction> CodeCommitFileActions { get; set; }

    public virtual DbSet<CodeCommitMetric> CodeCommitMetrics { get; set; }

    public virtual DbSet<CodeFeature> CodeFeatures { get; set; }

    public virtual DbSet<CodeRelease> CodeReleases { get; set; }

    public virtual DbSet<CodeReleaseFeature> CodeReleaseFeatures { get; set; }

    public virtual DbSet<CodeReview> CodeReviews { get; set; }

    public virtual DbSet<CodeReviewComment> CodeReviewComments { get; set; }

    public virtual DbSet<CodeReviewCommentCategory> CodeReviewCommentCategories { get; set; }

    public virtual DbSet<CodeReviewFile> CodeReviewFiles { get; set; }

    public virtual DbSet<DailyMeeting> DailyMeetings { get; set; }

    public virtual DbSet<DailyMeetingDiscussion> DailyMeetingDiscussions { get; set; }

    public virtual DbSet<DailyMeetingDiscussionParticipant> DailyMeetingDiscussionParticipants { get; set; }

    public virtual DbSet<DailyMeetingParticipant> DailyMeetingParticipants { get; set; }

    public virtual DbSet<Developer> Developers { get; set; }

    public virtual DbSet<DeveloperFeature> DeveloperFeatures { get; set; }

    public virtual DbSet<Epic> Epics { get; set; }

    public virtual DbSet<ParticipantEngagementLevel> ParticipantEngagementLevels { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectStakeHolder> ProjectStakeHolders { get; set; }

    public virtual DbSet<SeverityLevel> SeverityLevels { get; set; }

    public virtual DbSet<StakeHolder> StakeHolders { get; set; }

    public virtual DbSet<StakeHolderDepartment> StakeHolderDepartments { get; set; }

    public virtual DbSet<StakeHolderPosition> StakeHolderPositions { get; set; }

    public virtual DbSet<StaticAnalysisIssue> StaticAnalysisIssues { get; set; }

    public virtual DbSet<StaticAnalysisIssuesHistory> StaticAnalysisIssuesHistories { get; set; }

    public virtual DbSet<StaticAnalysisRule> StaticAnalysisRules { get; set; }

    public virtual DbSet<StaticAnalysisRuleCategory> StaticAnalysisRuleCategories { get; set; }

    public virtual DbSet<Vulnerabilite> Vulnerabilites { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=SoftwareAnalysis;user=root;password=1!Kigali", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.1.0-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Bug>(entity =>
        {
            entity.HasKey(e => e.BugKey).HasName("PRIMARY");

            entity.HasIndex(e => e.SeverityLevelId, "FK_Bug_Level");

            entity.Property(e => e.BugKey).HasMaxLength(36);
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(250);
            entity.Property(e => e.ReportedDatetimeUtc)
                .HasColumnType("datetime")
                .HasColumnName("ReportedDatetimeUTC");

            entity.HasOne(d => d.SeverityLevel).WithMany(p => p.Bugs)
                .HasForeignKey(d => d.SeverityLevelId)
                .HasConstraintName("FK_Bug_Level");
        });

        modelBuilder.Entity<CodeBase>(entity =>
        {
            entity.HasKey(e => e.CodeBaseKey).HasName("PRIMARY");

            entity.ToTable("CodeBase");

            entity.Property(e => e.CodeBaseKey).HasMaxLength(36);
            entity.Property(e => e.Framework).HasMaxLength(36);
            entity.Property(e => e.FrameworkVersion).HasMaxLength(10);
            entity.Property(e => e.Language)
                .IsRequired()
                .HasMaxLength(36);
            entity.Property(e => e.LanguageVersion).HasMaxLength(10);
        });

        modelBuilder.Entity<CodeBaseFile>(entity =>
        {
            entity.HasKey(e => e.CodeBaseFileId).HasName("PRIMARY");

            entity.HasIndex(e => e.CodeBaseKey, "FK_files_codebase");

            entity.Property(e => e.CodeBaseKey)
                .IsRequired()
                .HasMaxLength(36);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Filename)
                .IsRequired()
                .HasMaxLength(65);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CodeBaseKeyNavigation).WithMany(p => p.CodeBaseFiles)
                .HasForeignKey(d => d.CodeBaseKey)
                .HasConstraintName("FK_files_codebase");
        });

        modelBuilder.Entity<CodeBaseLibrary>(entity =>
        {
            entity.HasKey(e => e.CodeBaseLibraryId).HasName("PRIMARY");

            entity.HasIndex(e => e.CodeBaseKey, "FK_library_codebase");

            entity.Property(e => e.CodeBaseKey)
                .IsRequired()
                .HasMaxLength(36);
            entity.Property(e => e.LibraryName)
                .IsRequired()
                .HasMaxLength(150);
            entity.Property(e => e.LibraryVersion)
                .IsRequired()
                .HasMaxLength(10);

            entity.HasOne(d => d.CodeBaseKeyNavigation).WithMany(p => p.CodeBaseLibraries)
                .HasForeignKey(d => d.CodeBaseKey)
                .HasConstraintName("FK_library_codebase");
        });

        modelBuilder.Entity<CodeCommit>(entity =>
        {
            entity.HasKey(e => e.CodeCommitKey).HasName("PRIMARY");

            entity.HasIndex(e => e.CodeCommitMetricsKey, "FK_Feature_Metrics");

            entity.HasIndex(e => new { e.DeveloperFeatureId, e.CodeCommitMetricsKey }, "UC_feature_metrics").IsUnique();

            entity.Property(e => e.CodeCommitKey).HasMaxLength(36);
            entity.Property(e => e.CodeCommitMetricsKey)
                .IsRequired()
                .HasMaxLength(36);
            entity.Property(e => e.PreviousCommit).HasMaxLength(36);

            entity.HasOne(d => d.CodeCommitMetricsKeyNavigation).WithMany(p => p.CodeCommits)
                .HasForeignKey(d => d.CodeCommitMetricsKey)
                .HasConstraintName("FK_Feature_Metrics");

            entity.HasOne(d => d.DeveloperFeature).WithMany(p => p.CodeCommits)
                .HasForeignKey(d => d.DeveloperFeatureId)
                .HasConstraintName("FK_Commit_Feature");
        });

        modelBuilder.Entity<CodeCommitFile>(entity =>
        {
            entity.HasKey(e => e.CodeCommitFileId).HasName("PRIMARY");

            entity.HasIndex(e => e.CodeCommitFileActionId, "FK_file_action");

            entity.HasIndex(e => e.CodeBaseFileId, "FK_file_codebase");

            entity.HasIndex(e => e.CodeCommitKey, "FK_file_commit");

            entity.Property(e => e.CodeCommitKey)
                .IsRequired()
                .HasMaxLength(36);

            entity.HasOne(d => d.CodeBaseFile).WithMany(p => p.CodeCommitFiles)
                .HasForeignKey(d => d.CodeBaseFileId)
                .HasConstraintName("FK_file_codebase");

            entity.HasOne(d => d.CodeCommitFileAction).WithMany(p => p.CodeCommitFiles)
                .HasForeignKey(d => d.CodeCommitFileActionId)
                .HasConstraintName("FK_file_action");

            entity.HasOne(d => d.CodeCommitKeyNavigation).WithMany(p => p.CodeCommitFiles)
                .HasForeignKey(d => d.CodeCommitKey)
                .HasConstraintName("FK_file_commit");
        });

        modelBuilder.Entity<CodeCommitFileAction>(entity =>
        {
            entity.HasKey(e => e.CodeCommitFileActionId).HasName("PRIMARY");

            entity.Property(e => e.FileAction)
                .IsRequired()
                .HasMaxLength(36);
            entity.Property(e => e.FileActionDescription)
                .IsRequired()
                .HasMaxLength(250);
        });

        modelBuilder.Entity<CodeCommitMetric>(entity =>
        {
            entity.HasKey(e => e.CodeCommitMetricsKey).HasName("PRIMARY");

            entity.HasIndex(e => e.CommitDate, "idx_codecommit_date");

            entity.Property(e => e.CodeCommitMetricsKey).HasMaxLength(36);
            entity.Property(e => e.CodeCoveragePercentage).HasPrecision(4, 2);
            entity.Property(e => e.CommitComment)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.CommitDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CodeFeature>(entity =>
        {
            entity.HasKey(e => e.CodeFeatureKey).HasName("PRIMARY");

            entity.HasIndex(e => e.EpicKey, "FK_Feature_Epic");

            entity.HasIndex(e => e.CodeCompleteDate, "idx_codefeature_completedate");

            entity.HasIndex(e => e.CodeStartDate, "idx_codefeature_startdate");

            entity.Property(e => e.CodeFeatureKey).HasMaxLength(36);
            entity.Property(e => e.CodeCompleteDate).HasColumnType("datetime");
            entity.Property(e => e.CodeStartDate).HasColumnType("datetime");
            entity.Property(e => e.EpicKey)
                .IsRequired()
                .HasMaxLength(36);

            entity.HasOne(d => d.EpicKeyNavigation).WithMany(p => p.CodeFeatures)
                .HasForeignKey(d => d.EpicKey)
                .HasConstraintName("FK_Feature_Epic");
        });

        modelBuilder.Entity<CodeRelease>(entity =>
        {
            entity.HasKey(e => e.CodeReleaseKey).HasName("PRIMARY");

            entity.Property(e => e.CodeReleaseKey).HasMaxLength(36);
            entity.Property(e => e.ReleaseDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CodeReleaseFeature>(entity =>
        {
            entity.HasKey(e => e.CodeReleaseFeatureId).HasName("PRIMARY");

            entity.HasIndex(e => e.CodeFeatureKey, "FK_Feature_Release");

            entity.HasIndex(e => new { e.CodeReleaseKey, e.CodeFeatureKey }, "UC_release_feature").IsUnique();

            entity.Property(e => e.CodeFeatureKey)
                .IsRequired()
                .HasMaxLength(36);
            entity.Property(e => e.CodeReleaseKey)
                .IsRequired()
                .HasMaxLength(36);

            entity.HasOne(d => d.CodeFeatureKeyNavigation).WithMany(p => p.CodeReleaseFeatures)
                .HasForeignKey(d => d.CodeFeatureKey)
                .HasConstraintName("FK_Feature_Release");

            entity.HasOne(d => d.CodeReleaseKeyNavigation).WithMany(p => p.CodeReleaseFeatures)
                .HasForeignKey(d => d.CodeReleaseKey)
                .HasConstraintName("FK_Release_Feature");
        });

        modelBuilder.Entity<CodeReview>(entity =>
        {
            entity.HasKey(e => e.CodeReviewKey).HasName("PRIMARY");

            entity.HasIndex(e => e.DeveloperFeatureId, "idx_codereview_feature");

            entity.Property(e => e.CodeReviewKey).HasMaxLength(36);
            entity.Property(e => e.Approved).HasColumnType("bit(1)");
            entity.Property(e => e.ReviewCompleteDatetimeUtc)
                .HasColumnType("datetime")
                .HasColumnName("ReviewCompleteDatetimeUTC");
            entity.Property(e => e.ReviewStartDatetimeUtc)
                .HasColumnType("datetime")
                .HasColumnName("ReviewStartDatetimeUTC");

            entity.HasOne(d => d.DeveloperFeature).WithMany(p => p.CodeReviews)
                .HasForeignKey(d => d.DeveloperFeatureId)
                .HasConstraintName("FK_Review_Feature");
        });

        modelBuilder.Entity<CodeReviewComment>(entity =>
        {
            entity.HasKey(e => e.ReviewCommentId).HasName("PRIMARY");

            entity.HasIndex(e => e.CommentCategoryId, "FK_Review_CommentCategory");

            entity.HasIndex(e => e.CodeBaseFileId, "FK_Review_codebase");

            entity.HasIndex(e => e.CodeReviewKey, "idx_codereviewcomment_codereviewkey");

            entity.Property(e => e.CodeReviewKey)
                .IsRequired()
                .HasMaxLength(36);
            entity.Property(e => e.ReviewComment)
                .IsRequired()
                .HasMaxLength(500);

            entity.HasOne(d => d.CodeBaseFile).WithMany(p => p.CodeReviewComments)
                .HasForeignKey(d => d.CodeBaseFileId)
                .HasConstraintName("FK_Review_codebase");

            entity.HasOne(d => d.CommentCategory).WithMany(p => p.CodeReviewComments)
                .HasForeignKey(d => d.CommentCategoryId)
                .HasConstraintName("FK_Review_CommentCategory");
        });

        modelBuilder.Entity<CodeReviewCommentCategory>(entity =>
        {
            entity.HasKey(e => e.CommentCategoryId).HasName("PRIMARY");

            entity.Property(e => e.CategoryDescription)
                .IsRequired()
                .HasMaxLength(250);
            entity.Property(e => e.CommentCategory)
                .IsRequired()
                .HasMaxLength(150);
        });

        modelBuilder.Entity<CodeReviewFile>(entity =>
        {
            entity.HasKey(e => e.CodeReviewFileKey).HasName("PRIMARY");

            entity.HasIndex(e => e.CodeBaseFileId, "FK_reviewfiles_codebasefiles");

            entity.HasIndex(e => e.CodeReviewKey, "FK_reviewfiles_codereview");

            entity.Property(e => e.CodeReviewFileKey).HasMaxLength(36);
            entity.Property(e => e.CodeReviewKey)
                .IsRequired()
                .HasMaxLength(36);

            entity.HasOne(d => d.CodeBaseFile).WithMany(p => p.CodeReviewFiles)
                .HasForeignKey(d => d.CodeBaseFileId)
                .HasConstraintName("FK_reviewfiles_codebasefiles");

            entity.HasOne(d => d.CodeReviewKeyNavigation).WithMany(p => p.CodeReviewFiles)
                .HasForeignKey(d => d.CodeReviewKey)
                .HasConstraintName("FK_reviewfiles_codereview");
        });

        modelBuilder.Entity<DailyMeeting>(entity =>
        {
            entity.HasKey(e => e.DailyMeetingId).HasName("PRIMARY");

            entity.Property(e => e.MeetingEndDatetimeUtc)
                .HasColumnType("datetime")
                .HasColumnName("MeetingEndDatetimeUTC");
            entity.Property(e => e.MeetingStartDatetimeUtc)
                .HasColumnType("datetime")
                .HasColumnName("MeetingStartDatetimeUTC");
            entity.Property(e => e.MeetingSubject)
                .IsRequired()
                .HasMaxLength(500);
        });

        modelBuilder.Entity<DailyMeetingDiscussion>(entity =>
        {
            entity.HasKey(e => e.MeetingDiscussionId).HasName("PRIMARY");

            entity.HasIndex(e => e.DailyMeetingId, "idx_dailymeetingdiscussion_dailymeeting");

            entity.Property(e => e.DiscussionSummary)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.DiscussionTopic)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasOne(d => d.DailyMeeting).WithMany(p => p.DailyMeetingDiscussions)
                .HasForeignKey(d => d.DailyMeetingId)
                .HasConstraintName("FK_Discussion_Meeting");
        });

        modelBuilder.Entity<DailyMeetingDiscussionParticipant>(entity =>
        {
            entity.HasKey(e => e.DiscussionParticipantId).HasName("PRIMARY");

            entity.HasIndex(e => e.MeetingDiscussionId, "FK_DiscussionParticipant_Discussion");

            entity.HasIndex(e => e.MeetingParticipantId, "FK_DiscussionParticipant_MeetingParticipant");

            entity.Property(e => e.ParticipantDiscussionView)
                .IsRequired()
                .HasMaxLength(250);

            entity.HasOne(d => d.MeetingDiscussion).WithMany(p => p.DailyMeetingDiscussionParticipants)
                .HasForeignKey(d => d.MeetingDiscussionId)
                .HasConstraintName("FK_DiscussionParticipant_Discussion");

            entity.HasOne(d => d.MeetingParticipant).WithMany(p => p.DailyMeetingDiscussionParticipants)
                .HasForeignKey(d => d.MeetingParticipantId)
                .HasConstraintName("FK_DiscussionParticipant_MeetingParticipant");
        });

        modelBuilder.Entity<DailyMeetingParticipant>(entity =>
        {
            entity.HasKey(e => e.MeetingParticipantId).HasName("PRIMARY");

            entity.HasIndex(e => e.EngagementLevelId, "FK_Meeting_Engagement");

            entity.HasIndex(e => e.DeveloperKey, "idx_meetingparticipants_developer");

            entity.HasIndex(e => e.StakeHolderKey, "idx_meetingparticipants_stakeholder");

            entity.Property(e => e.DeveloperKey).HasMaxLength(36);
            entity.Property(e => e.StakeHolderKey).HasMaxLength(36);

            entity.HasOne(d => d.DeveloperKeyNavigation).WithMany(p => p.DailyMeetingParticipants)
                .HasForeignKey(d => d.DeveloperKey)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Meeting_Developer");

            entity.HasOne(d => d.EngagementLevel).WithMany(p => p.DailyMeetingParticipants)
                .HasForeignKey(d => d.EngagementLevelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Meeting_Engagement");

            entity.HasOne(d => d.StakeHolderKeyNavigation).WithMany(p => p.DailyMeetingParticipants)
                .HasForeignKey(d => d.StakeHolderKey)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Meeting_StakeHolder");
        });

        modelBuilder.Entity<Developer>(entity =>
        {
            entity.HasKey(e => e.DeveloperKey).HasName("PRIMARY");

            entity.Property(e => e.DeveloperKey).HasMaxLength(36);
        });

        modelBuilder.Entity<DeveloperFeature>(entity =>
        {
            entity.HasKey(e => e.DeveloperFeatureId).HasName("PRIMARY");

            entity.HasIndex(e => new { e.DeveloperKey, e.CodeFeatureKey }, "UC_developer_feature").IsUnique();

            entity.HasIndex(e => e.CodeFeatureKey, "UC_feature").IsUnique();

            entity.Property(e => e.CodeFeatureKey)
                .IsRequired()
                .HasMaxLength(36);
            entity.Property(e => e.DeveloperKey)
                .IsRequired()
                .HasMaxLength(36);

            entity.HasOne(d => d.CodeFeatureKeyNavigation).WithOne(p => p.DeveloperFeature)
                .HasForeignKey<DeveloperFeature>(d => d.CodeFeatureKey)
                .HasConstraintName("FK_Feature_Developer");

            entity.HasOne(d => d.DeveloperKeyNavigation).WithMany(p => p.DeveloperFeatures)
                .HasForeignKey(d => d.DeveloperKey)
                .HasConstraintName("FK_Developer_Feature");
        });

        modelBuilder.Entity<Epic>(entity =>
        {
            entity.HasKey(e => e.EpicKey).HasName("PRIMARY");

            entity.HasIndex(e => e.ProjectKey, "FK_Epic_Project");

            entity.Property(e => e.EpicKey).HasMaxLength(36);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ProjectKey)
                .IsRequired()
                .HasMaxLength(36);

            entity.HasOne(d => d.ProjectKeyNavigation).WithMany(p => p.Epics)
                .HasForeignKey(d => d.ProjectKey)
                .HasConstraintName("FK_Epic_Project");
        });

        modelBuilder.Entity<ParticipantEngagementLevel>(entity =>
        {
            entity.HasKey(e => e.EngagementLevelId).HasName("PRIMARY");

            entity.Property(e => e.EngagementCategory)
                .IsRequired()
                .HasMaxLength(150);
            entity.Property(e => e.EngagementDescription)
                .IsRequired()
                .HasMaxLength(250);
            entity.Property(e => e.EngagementRank).HasComment("The lower the number, the more desireable.");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectKey).HasName("PRIMARY");

            entity.Property(e => e.ProjectKey).HasMaxLength(36);
            entity.Property(e => e.KickoffDate).HasColumnType("datetime");
            entity.Property(e => e.ProjectRepositoryUrl)
                .HasMaxLength(150)
                .HasColumnName("ProjectRepositoryURL");
            entity.Property(e => e.ProjectedDeliveryDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectStakeHolder>(entity =>
        {
            entity.HasKey(e => e.ProjectStakeHolderId).HasName("PRIMARY");

            entity.HasIndex(e => e.StakeHolderKey, "FK_Project_StakeHolder");

            entity.HasIndex(e => new { e.ProjectKey, e.StakeHolderKey }, "UC_project_stakeholder").IsUnique();

            entity.Property(e => e.ProjectKey)
                .IsRequired()
                .HasMaxLength(36);
            entity.Property(e => e.StakeHolderKey)
                .IsRequired()
                .HasMaxLength(36);

            entity.HasOne(d => d.ProjectKeyNavigation).WithMany(p => p.ProjectStakeHolders)
                .HasForeignKey(d => d.ProjectKey)
                .HasConstraintName("FK_StakeHolder_Project");

            entity.HasOne(d => d.StakeHolderKeyNavigation).WithMany(p => p.ProjectStakeHolders)
                .HasForeignKey(d => d.StakeHolderKey)
                .HasConstraintName("FK_Project_StakeHolder");
        });

        modelBuilder.Entity<SeverityLevel>(entity =>
        {
            entity.HasKey(e => e.SeverityLevelId).HasName("PRIMARY");

            entity.Property(e => e.SeverityCategory)
                .IsRequired()
                .HasMaxLength(150);
            entity.Property(e => e.SeverityDescription)
                .IsRequired()
                .HasMaxLength(250);
        });

        modelBuilder.Entity<StakeHolder>(entity =>
        {
            entity.HasKey(e => e.StakeHolderKey).HasName("PRIMARY");

            entity.HasIndex(e => e.StakeHolderPositionId, "FK_Stakeholder_Position");

            entity.HasIndex(e => new { e.StakeHolderDepartmentId, e.StakeHolderPositionId }, "UC_department_position").IsUnique();

            entity.Property(e => e.StakeHolderKey).HasMaxLength(36);

            entity.HasOne(d => d.StakeHolderDepartment).WithMany(p => p.StakeHolders)
                .HasForeignKey(d => d.StakeHolderDepartmentId)
                .HasConstraintName("FK_Stakeholder_department");

            entity.HasOne(d => d.StakeHolderPosition).WithMany(p => p.StakeHolders)
                .HasForeignKey(d => d.StakeHolderPositionId)
                .HasConstraintName("FK_Stakeholder_Position");
        });

        modelBuilder.Entity<StakeHolderDepartment>(entity =>
        {
            entity.HasKey(e => e.StakeHolderDepartmentId).HasName("PRIMARY");

            entity.HasIndex(e => e.DepartmentName, "UC_department_department").IsUnique();

            entity.Property(e => e.DepartmentName)
                .IsRequired()
                .HasMaxLength(150);
        });

        modelBuilder.Entity<StakeHolderPosition>(entity =>
        {
            entity.HasKey(e => e.StakeHolderPositionId).HasName("PRIMARY");

            entity.Property(e => e.HasManagementDuties).HasColumnType("bit(1)");
            entity.Property(e => e.Position)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.PositionDescription)
                .IsRequired()
                .HasMaxLength(250);
        });

        modelBuilder.Entity<StaticAnalysisIssue>(entity =>
        {
            entity.HasKey(e => e.IssueId).HasName("PRIMARY");

            entity.HasIndex(e => e.CodeBaseFileId, "FK_issues_codebasefiles");

            entity.HasIndex(e => e.ProjectKey, "FK_issues_project");

            entity.HasIndex(e => e.RuleId, "FK_issues_rules");

            entity.HasIndex(e => e.SeverityLevelId, "FK_issues_severity");

            entity.Property(e => e.DateFound).HasColumnType("datetime");
            entity.Property(e => e.DateResolved).HasColumnType("datetime");
            entity.Property(e => e.IssueDescription).HasMaxLength(250);
            entity.Property(e => e.ProjectKey)
                .IsRequired()
                .HasMaxLength(36);

            entity.HasOne(d => d.CodeBaseFile).WithMany(p => p.StaticAnalysisIssues)
                .HasForeignKey(d => d.CodeBaseFileId)
                .HasConstraintName("FK_issues_codebasefiles");

            entity.HasOne(d => d.ProjectKeyNavigation).WithMany(p => p.StaticAnalysisIssues)
                .HasForeignKey(d => d.ProjectKey)
                .HasConstraintName("FK_issues_project");

            entity.HasOne(d => d.Rule).WithMany(p => p.StaticAnalysisIssues)
                .HasForeignKey(d => d.RuleId)
                .HasConstraintName("FK_issues_rules");

            entity.HasOne(d => d.SeverityLevel).WithMany(p => p.StaticAnalysisIssues)
                .HasForeignKey(d => d.SeverityLevelId)
                .HasConstraintName("FK_issues_severity");
        });

        modelBuilder.Entity<StaticAnalysisIssuesHistory>(entity =>
        {
            entity.HasKey(e => e.IssueHistoryId).HasName("PRIMARY");

            entity.ToTable("StaticAnalysisIssuesHistory");

            entity.HasIndex(e => e.ProjectKey, "FK_issueshistory_project");

            entity.Property(e => e.AnalysisDate).HasColumnType("datetime");
            entity.Property(e => e.ProjectKey)
                .IsRequired()
                .HasMaxLength(36);

            entity.HasOne(d => d.ProjectKeyNavigation).WithMany(p => p.StaticAnalysisIssuesHistories)
                .HasForeignKey(d => d.ProjectKey)
                .HasConstraintName("FK_issueshistory_project");
        });

        modelBuilder.Entity<StaticAnalysisRule>(entity =>
        {
            entity.HasKey(e => e.RuleId).HasName("PRIMARY");

            entity.HasIndex(e => e.RuleCategoryId, "FK_rules_category");

            entity.Property(e => e.RuleCode)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Unique code for the rule, often provided by static analysis tools");
            entity.Property(e => e.RuleDescription)
                .IsRequired()
                .HasMaxLength(250);

            entity.HasOne(d => d.RuleCategory).WithMany(p => p.StaticAnalysisRules)
                .HasForeignKey(d => d.RuleCategoryId)
                .HasConstraintName("FK_rules_category");
        });

        modelBuilder.Entity<StaticAnalysisRuleCategory>(entity =>
        {
            entity.HasKey(e => e.RuleCategoryId).HasName("PRIMARY");

            entity.Property(e => e.RuleCategoryDescription)
                .IsRequired()
                .HasMaxLength(250);
        });

        modelBuilder.Entity<Vulnerabilite>(entity =>
        {
            entity.HasKey(e => e.VulnetabilityId).HasName("PRIMARY");

            entity.Property(e => e.Framework).HasMaxLength(36);
            entity.Property(e => e.FrameworkVersion).HasMaxLength(10);
            entity.Property(e => e.LibraryName).HasMaxLength(150);
            entity.Property(e => e.LibraryVersion).HasMaxLength(10);
            entity.Property(e => e.ResolvingVersion).HasMaxLength(10);
            entity.Property(e => e.VulnerabilityDescription)
                .IsRequired()
                .HasMaxLength(250);
            entity.Property(e => e.VulnerabiltyName)
                .IsRequired()
                .HasMaxLength(150);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
