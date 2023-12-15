DROP DATABASE IF EXISTS SoftwareAnalysis;
CREATE DATABASE SoftwareAnalysis;
USE SoftwareAnalysis;


/********************************************* TABLE CREATION *******************************************************/
DROP TABLE IF EXISTS Projects;
CREATE TABLE Projects(
	ProjectKey VARCHAR(36) PRIMARY KEY,
	KickoffDate datetime not null,
	ProjectedDeliveryDate datetime not null,
	ProjectRepositoryURL VARCHAR(150) null
);

DROP TABLE IF EXISTS StakeHolders;
CREATE TABLE StakeHolders(
	StakeHolderKey VARCHAR(36) PRIMARY KEY,
	StakeHolderDepartmentId int not null,
	StakeHolderPositionId int not null
);

DROP TABLE IF EXISTS StakeHolderDepartments;
CREATE TABLE StakeHolderDepartments(
	StakeHolderDepartmentId int not null AUTO_INCREMENT PRIMARY KEY,
	DepartmentName varchar(150) not null
);

DROP TABLE IF EXISTS StakeHolderPositions;
CREATE TABLE StakeHolderPositions(
	StakeHolderPositionId int not null AUTO_INCREMENT PRIMARY KEY,
	Position varchar(50) not null,
	HasManagementDuties bit not null,
	PositionDescription varchar(250) not null
);


DROP TABLE IF EXISTS ProjectStakeHolders;
CREATE TABLE ProjectStakeHolders(
	ProjectStakeHolderId int not null AUTO_INCREMENT PRIMARY KEY,
	ProjectKey VARCHAR(36) not null,
	StakeHolderKey VARCHAR(36) not null
);


DROP TABLE IF EXISTS Epics;
CREATE TABLE Epics(
	EpicKey VARCHAR(36) PRIMARY KEY,
	ProjectKey VARCHAR(36) not null,
	CreatedDate datetime not null

);

DROP TABLE IF EXISTS CodeBase;
CREATE TABLE CodeBase(
	CodeBaseKey VARCHAR(36) PRIMARY KEY,
	Language varchar(36) not null,
	LanguageVersion VARCHAR(10) null,
	Framework varchar(36) null,
	FrameworkVersion VARCHAR(10) null
);

DROP TABLE IF EXISTS CodeBaseLibraries;
CREATE TABLE CodeBaseLibraries(
	CodeBaseLibraryId int not null AUTO_INCREMENT PRIMARY KEY,
	CodeBaseKey VARCHAR(36) not null,
	LibraryName VARCHAR(150) not null,
	LibraryVersion VARCHAR(10) not null
);

DROP TABLE IF EXISTS CodeBaseFiles;
CREATE TABLE CodeBaseFiles(
	CodeBaseFileId int not null AUTO_INCREMENT PRIMARY KEY,
	CodeBaseKey VARCHAR(36) not null,
	Filename VARCHAR(65) not null,
	CreatedDate datetime not null,
	UpdatedDate datetime null
);

DROP TABLE IF EXISTS CodeFeatures;
CREATE TABLE CodeFeatures(
	CodeFeatureKey VARCHAR(36) PRIMARY KEY,
	EpicKey VARCHAR(36) not null,
	CodeStartDate datetime null,
	CodeCompleteDate datetime null
);


DROP TABLE IF EXISTS CodeCommits;
CREATE TABLE CodeCommits(
	CodeCommitKey VARCHAR(36) PRIMARY KEY,
	PreviousCommit VARCHAR(36)  null,
	DeveloperFeatureId int not null,
	CodeCommitMetricsKey VARCHAR(36) not null
);

DROP TABLE IF EXISTS CodeCommitFiles;
CREATE TABLE CodeCommitFiles(
	CodeCommitFileId int not null AUTO_INCREMENT PRIMARY KEY,
	CodeCommitKey VARCHAR(36) not null,
	CodeBaseFileId int not null,
	CodeCommitFileActionId int not null
);

DROP TABLE IF EXISTS CodeCommitFileActions;
CREATE TABLE CodeCommitFileActions(
	CodeCommitFileActionId int not null AUTO_INCREMENT PRIMARY KEY,
	FileAction VARCHAR(36) not null,
	FileActionDescription VARCHAR(250) not null
);


DROP TABLE IF EXISTS CodeReleases;
CREATE TABLE CodeReleases(
	CodeReleaseKey VARCHAR(36) PRIMARY KEY,
	ReleaseDate datetime not null
);

DROP TABLE IF EXISTS CodeReleaseFeatures;
CREATE TABLE CodeReleaseFeatures(
	CodeReleaseFeatureId int not null AUTO_INCREMENT PRIMARY KEY,
	CodeReleaseKey VARCHAR(36) not null,
	CodeFeatureKey VARCHAR(36) not null
);


DROP TABLE IF EXISTS Developers;
CREATE TABLE Developers(
	DeveloperKey VARCHAR(36) PRIMARY KEY,
	YearsOfExpirience int not null
);


DROP TABLE IF EXISTS DeveloperFeatures;
CREATE TABLE DeveloperFeatures(
	DeveloperFeatureId int not null AUTO_INCREMENT PRIMARY KEY,
	DeveloperKey VARCHAR(36) not null,
	CodeFeatureKey VARCHAR(36) not null
);


DROP TABLE IF EXISTS CodeCommitMetrics;
CREATE TABLE CodeCommitMetrics(
	CodeCommitMetricsKey VARCHAR(36) PRIMARY KEY,
	LinesOfCode int null,
	CodeCoveragePercentage decimal(4,2) null,
	CompletionTimeInMinutes int null,
	CommitDate datetime not null,
	CommitComment varchar(500) not null
);

DROP TABLE IF EXISTS CodeReviews;
CREATE TABLE CodeReviews(
	CodeReviewKey VARCHAR(36) PRIMARY KEY,
	DeveloperFeatureId int not null,
	Approved bit not null,
	ReviewStartDatetimeUTC datetime not null,
	ReviewCompleteDatetimeUTC datetime null
);

DROP TABLE IF EXISTS CodeReviewFiles;
CREATE TABLE CodeReviewFiles(
	CodeReviewFileKey VARCHAR(36) PRIMARY KEY,
	CodeReviewKey VARCHAR(36) not null,
	CodeBaseFileId int not null
);

DROP TABLE IF EXISTS CodeReviewComments;
CREATE TABLE CodeReviewComments(
	ReviewCommentId int not null AUTO_INCREMENT PRIMARY KEY,
	CodeReviewKey VARCHAR(36) not null,
	CommentCategoryId int not null,
	CodeBaseFileId int not null,
	StartingFromLineNumber int not null,
	EndingAtLineNumber int not null,
	ReviewComment varchar(500) not null
);


DROP TABLE IF EXISTS CodeReviewCommentCategories;
CREATE TABLE CodeReviewCommentCategories(
	CommentCategoryId int not null AUTO_INCREMENT PRIMARY KEY,
	CommentCategory varchar(150) not null,
	CategoryDescription varchar(250) not null
);


DROP TABLE IF EXISTS DailyMeetings;
CREATE TABLE DailyMeetings(
	DailyMeetingId int AUTO_INCREMENT PRIMARY KEY,
	MeetingStartDatetimeUTC datetime not null,
	MeetingEndDatetimeUTC datetime not null,
	MeetingSubject varchar(500) not null
);


DROP TABLE IF EXISTS DailyMeetingParticipants;
CREATE TABLE DailyMeetingParticipants(
	MeetingParticipantId int not null AUTO_INCREMENT PRIMARY KEY,
	DeveloperKey VARCHAR(36) null,
	StakeHolderKey VARCHAR(36) null,
	EngagementLevelId int null

);


DROP TABLE IF EXISTS ParticipantEngagementLevels;
CREATE TABLE ParticipantEngagementLevels(
	EngagementLevelId int not null AUTO_INCREMENT PRIMARY KEY,
	EngagementCategory varchar(150) not null,
	EngagementDescription varchar(250) not null,
	EngagementRank int not null COMMENT 'The lower the number, the more desireable.'
);

DROP TABLE IF EXISTS DailyMeetingDiscussions;
CREATE TABLE DailyMeetingDiscussions(
	MeetingDiscussionId int not null AUTO_INCREMENT PRIMARY KEY,
	DailyMeetingId int not null,
	DiscussionTopic varchar(150) not null,
	DiscussionSummary varchar(500) not null
);


DROP TABLE IF EXISTS DailyMeetingDiscussionParticipants;
CREATE TABLE DailyMeetingDiscussionParticipants(
	DiscussionParticipantId int not null AUTO_INCREMENT PRIMARY KEY,
	MeetingParticipantId int not null,
	MeetingDiscussionId int not null,
	ParticipantDiscussionView varchar(250) not null
);

DROP TABLE IF EXISTS Vulnerabilites;
CREATE TABLE Vulnerabilites(
	VulnetabilityId int not null AUTO_INCREMENT PRIMARY KEY,
	Framework VARCHAR(36) null,
	FrameworkVersion VARCHAR(10) null,
	LibraryName VARCHAR(150) null,
	LibraryVersion VARCHAR(10) null,
	VulnerabiltyName VARCHAR(150) not null,
	VulnerabilityDescription VARCHAR(250) not null,
	ResolvingVersion VARCHAR(10) null
);


DROP TABLE IF EXISTS Bugs;
CREATE TABLE Bugs(
	BugKey VARCHAR(36) PRIMARY KEY,
	ReportedDatetimeUTC datetime not null,
	Description varchar(250) not null,
	SeverityLevelId int not null
);

DROP TABLE IF EXISTS SeverityLevels;
CREATE TABLE SeverityLevels(
	SeverityLevelId int not null AUTO_INCREMENT PRIMARY KEY,
	SeverityCategory varchar(150) not null,
	SeverityDescription varchar(250) not null
);

DROP TABLE IF EXISTS StaticAnalysisIssues;
CREATE TABLE StaticAnalysisIssues(
	IssueId int not null AUTO_INCREMENT PRIMARY KEY,
	ProjectKey VARCHAR(36) not null,
	RuleId int not null,
	CodeBaseFileId int not null,
	LineNumber int not null,
	ColumnNumber int not null,
	IssueDescription VARCHAR(250),
	SeverityLevelId int not null,
	DateFound datetime not null,
	DateResolved datetime null
);

DROP TABLE IF EXISTS StaticAnalysisRuleCategories;
CREATE TABLE StaticAnalysisRuleCategories(
	RuleCategoryId int not null AUTO_INCREMENT PRIMARY KEY,
	RuleCategoryDescription VARCHAR(250) not null
);

DROP TABLE IF EXISTS StaticAnalysisRules;
CREATE TABLE StaticAnalysisRules(
	RuleId int not null AUTO_INCREMENT PRIMARY KEY,
	RuleCode VARCHAR(50) not null COMMENT 'Unique code for the rule, often provided by static analysis tools',
	RuleDescription VARCHAR(250) not null,
	RuleCategoryId int not null
);

DROP TABLE IF EXISTS StaticAnalysisIssuesHistory;
CREATE TABLE StaticAnalysisIssuesHistory(
	IssueHistoryId int not null AUTO_INCREMENT PRIMARY KEY,
	ProjectKey VARCHAR(36) not null,
	AnalysisDate datetime not null,
	TotalIssuesFound int not null,
	TotalCriticalIssues int null,
	TotalHighSeverityIssues int null,
	TotalMediumSeverityIssues int null,
	TotalLowSeverityIssues int null
);

/********************************************* FOREIGN KEY CONSTRAINTS ************************************************/

ALTER TABLE ProjectStakeHolders
	ADD CONSTRAINT FK_Project_StakeHolder 
		FOREIGN KEY (StakeHolderKey) REFERENCES StakeHolders(StakeHolderKey) 
	ON DELETE CASCADE;

ALTER TABLE ProjectStakeHolders 
	ADD CONSTRAINT FK_StakeHolder_Project 
		FOREIGN KEY (ProjectKey) REFERENCES Projects(ProjectKey) 
	ON DELETE CASCADE;

ALTER TABLE StakeHolders 
	ADD CONSTRAINT FK_Stakeholder_department 
		FOREIGN KEY (StakeHolderDepartmentId) REFERENCES StakeHolderDepartments(StakeHolderDepartmentId) 
	ON DELETE CASCADE;

ALTER TABLE StakeHolders 
	ADD CONSTRAINT FK_Stakeholder_Position 
		FOREIGN KEY (StakeHolderPositionId) REFERENCES StakeHolderPositions(StakeHolderPositionId) 
	ON DELETE CASCADE;


ALTER TABLE Epics 
	ADD CONSTRAINT FK_Epic_Project 
		FOREIGN KEY (ProjectKey) REFERENCES Projects(ProjectKey) 
	ON DELETE CASCADE;


ALTER TABLE CodeFeatures 
	ADD CONSTRAINT FK_Feature_Epic 
		FOREIGN KEY (EpicKey) REFERENCES Epics(EpicKey) 
	ON DELETE CASCADE;


ALTER TABLE CodeCommits 
	ADD CONSTRAINT FK_Commit_Feature 
		FOREIGN KEY (DeveloperFeatureId) REFERENCES DeveloperFeatures(DeveloperFeatureId) 
	ON DELETE CASCADE;

ALTER TABLE CodeCommits 
	ADD CONSTRAINT FK_Feature_Metrics 
		FOREIGN KEY (CodeCommitMetricsKey) REFERENCES CodeCommitMetrics(CodeCommitMetricsKey) 
	ON DELETE CASCADE;


ALTER TABLE CodeReleaseFeatures 
	ADD CONSTRAINT FK_Release_Feature 
		FOREIGN KEY (CodeReleaseKey) REFERENCES CodeReleases(CodeReleaseKey) 
	ON DELETE CASCADE;

ALTER TABLE CodeReleaseFeatures 
	ADD CONSTRAINT FK_Feature_Release 
		FOREIGN KEY (CodeFeatureKey) REFERENCES CodeFeatures(CodeFeatureKey) 
	ON DELETE CASCADE;


ALTER TABLE DeveloperFeatures 
	ADD CONSTRAINT FK_Developer_Feature 
		FOREIGN KEY (DeveloperKey) REFERENCES Developers(DeveloperKey) 
	ON DELETE CASCADE;

ALTER TABLE DeveloperFeatures 
	ADD CONSTRAINT FK_Feature_Developer 
		FOREIGN KEY (CodeFeatureKey) REFERENCES CodeFeatures(CodeFeatureKey) 
	ON DELETE CASCADE;



ALTER TABLE CodeReviews 
	ADD CONSTRAINT FK_Review_Feature 
		FOREIGN KEY (DeveloperFeatureId) REFERENCES DeveloperFeatures(DeveloperFeatureId) 
	ON DELETE CASCADE;


ALTER TABLE CodeReviewComments 
	ADD CONSTRAINT FK_Review_CommentCategory 
		FOREIGN KEY (CommentCategoryId) REFERENCES CodeReviewCommentCategories(CommentCategoryId) 
	ON DELETE CASCADE;

ALTER TABLE CodeReviewComments 
	ADD CONSTRAINT FK_Review_codebase
		FOREIGN KEY (CodeBaseFileId) REFERENCES CodeBaseFiles(CodeBaseFileId) 
	ON DELETE CASCADE;

ALTER TABLE DailyMeetingParticipants 
	ADD CONSTRAINT FK_Meeting_Developer 
		FOREIGN KEY (DeveloperKey) REFERENCES Developers(DeveloperKey) 
	ON DELETE CASCADE;

ALTER TABLE DailyMeetingParticipants 
	ADD CONSTRAINT FK_Meeting_StakeHolder 
		FOREIGN KEY (StakeHolderKey) REFERENCES StakeHolders(StakeHolderKey) 
	ON DELETE CASCADE;

ALTER TABLE DailyMeetingParticipants 
	ADD CONSTRAINT FK_Meeting_Engagement 
		FOREIGN KEY (EngagementLevelId) REFERENCES ParticipantEngagementLevels(EngagementLevelId) 
	ON DELETE CASCADE;


ALTER TABLE Bugs 
	ADD CONSTRAINT FK_Bug_Level 
		FOREIGN KEY (SeverityLevelId) REFERENCES SeverityLevels(SeverityLevelId) 
	ON DELETE CASCADE;


ALTER TABLE DailyMeetingDiscussions 
	ADD CONSTRAINT FK_Discussion_Meeting 
		FOREIGN KEY (DailyMeetingId) REFERENCES DailyMeetings(DailyMeetingId) 
	ON DELETE CASCADE;


ALTER TABLE DailyMeetingDiscussionParticipants 
	ADD CONSTRAINT FK_DiscussionParticipant_Discussion 
		FOREIGN KEY (MeetingDiscussionId) REFERENCES DailyMeetingDiscussions(MeetingDiscussionId) 
	ON DELETE CASCADE;

ALTER TABLE DailyMeetingDiscussionParticipants 
	ADD CONSTRAINT FK_DiscussionParticipant_MeetingParticipant 
		FOREIGN KEY (MeetingParticipantId) REFERENCES DailyMeetingParticipants(MeetingParticipantId) 
	ON DELETE CASCADE;

ALTER TABLE CodeReviewFiles
	ADD CONSTRAINT FK_reviewfiles_codereview
		FOREIGN KEY (CodeReviewKey) REFERENCES CodeReviews(CodeReviewKey)
	ON DELETE CASCADE;

ALTER TABLE CodeReviewFiles
	ADD CONSTRAINT FK_reviewfiles_codebasefiles
		FOREIGN KEY (CodeBaseFileId) REFERENCES CodeBaseFiles(CodeBaseFileId)
	ON DELETE CASCADE;

ALTER TABLE CodeBaseFiles
	ADD CONSTRAINT FK_files_codebase
		FOREIGN KEY (CodeBaseKey) REFERENCES CodeBase(CodeBaseKey)
	ON DELETE CASCADE;

ALTER TABLE CodeCommitFiles
	ADD CONSTRAINT FK_file_commit
		FOREIGN KEY (CodeCommitKey) REFERENCES CodeCommits(CodeCommitKey)
	ON DELETE CASCADE;

ALTER TABLE CodeCommitFiles
	ADD CONSTRAINT FK_file_codebase
		FOREIGN KEY (CodeBaseFileId) REFERENCES CodeBaseFiles(CodeBaseFileId)
	ON DELETE CASCADE;

ALTER TABLE CodeCommitFiles
	ADD CONSTRAINT FK_file_action
		FOREIGN KEY (CodeCommitFileActionId) REFERENCES CodeCommitFileActions(CodeCommitFileActionId)
	ON DELETE CASCADE;

ALTER TABLE CodeBaseLibraries
	ADD CONSTRAINT FK_library_codebase
		FOREIGN KEY (CodeBaseKey) REFERENCES CodeBase(CodeBaseKey)
	ON DELETE CASCADE;

ALTER TABLE StaticAnalysisIssuesHistory
	ADD CONSTRAINT FK_issueshistory_project
		FOREIGN KEY (ProjectKey) REFERENCES Projects(ProjectKey)
	ON DELETE CASCADE;

ALTER TABLE StaticAnalysisRules
	ADD CONSTRAINT FK_rules_category
		FOREIGN KEY (RuleCategoryId) REFERENCES StaticAnalysisRuleCategories(RuleCategoryId)
	ON DELETE CASCADE;

ALTER TABLE StaticAnalysisIssues
	ADD CONSTRAINT FK_issues_project
		FOREIGN KEY (ProjectKey) REFERENCES Projects(ProjectKey)
	ON DELETE CASCADE;

ALTER TABLE StaticAnalysisIssues
	ADD CONSTRAINT FK_issues_rules
		FOREIGN KEY (RuleId) REFERENCES StaticAnalysisRules(RuleId)
	ON DELETE CASCADE;

ALTER TABLE StaticAnalysisIssues
	ADD CONSTRAINT FK_issues_codebasefiles
		FOREIGN KEY (CodeBaseFileId) REFERENCES CodeBaseFiles(CodeBaseFileId)
	ON DELETE CASCADE;

ALTER TABLE StaticAnalysisIssues
	ADD CONSTRAINT FK_issues_severity
		FOREIGN KEY (SeverityLevelId) REFERENCES SeverityLevels(SeverityLevelId)
	ON DELETE CASCADE;


/********************************************* DATA VALIDATION CONSTRAINTS *******************************************/

ALTER TABLE DailyMeetingParticipants
	ADD CONSTRAINT developer_or_stackholder
	CHECK (
		(DeveloperKey IS NULL AND StakeHolderKey IS NOT NULL) OR
		(StakeHolderKey IS NULL AND DeveloperKey IS NOT NULL)
);

ALTER TABLE DeveloperFeatures
	ADD CONSTRAINT UC_developer_feature
	UNIQUE (DeveloperKey, CodeFeatureKey);

ALTER TABLE DeveloperFeatures
	ADD CONSTRAINT UC_feature
	UNIQUE (CodeFeatureKey);

ALTER TABLE CodeCommits
	ADD CONSTRAINT UC_feature_metrics
	UNIQUE (DeveloperFeatureId, CodeCommitMetricsKey);

ALTER TABLE CodeReleaseFeatures
	ADD CONSTRAINT UC_release_feature
	UNIQUE (CodeReleaseKey, CodeFeatureKey);

ALTER TABLE StakeHolders
	ADD CONSTRAINT UC_department_position
	UNIQUE (StakeHolderDepartmentId, StakeHolderPositionId);

ALTER TABLE StakeHolderDepartments
	ADD CONSTRAINT UC_department_department
	UNIQUE (DepartmentName);

ALTER TABLE ProjectStakeHolders
	ADD CONSTRAINT UC_project_stakeholder
	UNIQUE (ProjectKey, StakeHolderKey);

/********************************************* INDEX CONSTRAINTS *****************************************************/

CREATE INDEX idx_codecommit_date ON CodeCommitMetrics (CommitDate);

CREATE INDEX idx_codefeature_startdate ON CodeFeatures (CodeStartDate);
CREATE INDEX idx_codefeature_completedate ON CodeFeatures (CodeCompleteDate);

CREATE INDEX idx_codereview_feature ON CodeReviews (DeveloperFeatureId);

CREATE INDEX idx_codereviewcomment_codereviewkey ON CodeReviewComments(CodeReviewKey);

CREATE INDEX idx_meetingparticipants_developer ON DailyMeetingParticipants(DeveloperKey);
CREATE INDEX idx_meetingparticipants_stakeholder ON DailyMeetingParticipants(StakeHolderKey);

CREATE INDEX idx_dailymeetingdiscussion_dailymeeting ON DailyMeetingDiscussions(DailyMeetingId);


/********************************************* TRIGGER CONSTRAINTS ***************************************************/

DELIMITER $$

CREATE TRIGGER check_commit_date_insert
BEFORE INSERT ON CodeCommits
FOR EACH ROW
	BEGIN
        IF(
			select 
			Case 
				when ccm.CommitDate > cf.CodeCompleteDate then 1
				else 0
			end
			from CodeCommits cc
				Join CodeCommitMetrics ccm on ccm.CodeCommitMetricsKey = cc.CodeCommitMetricsKey
				Join DeveloperFeatures df on df.DeveloperFeatureId = cc.DeveloperFeatureId
				Join CodeFeatures cf on cf.CodeFeatureKey = df.CodeFeatureKey
			where cc.CodeCommitMetricsKey = NEW.CodeCommitMetricsKey
        ) THEN
			SIGNAL SQLSTATE '45000'
			SET MESSAGE_TEXT = 'Code commit data must be less that feature code complete date';
        END IF;
	END;

CREATE TRIGGER check_commit_date_update
BEFORE UPDATE ON CodeCommits
FOR EACH ROW
	BEGIN
        IF(
			select 
			Case 
				when ccm.CommitDate > cf.CodeCompleteDate then 1
				else 0
			end
			from CodeCommits cc
				Join CodeCommitMetrics ccm on ccm.CodeCommitMetricsKey = cc.CodeCommitMetricsKey
				Join DeveloperFeatures df on df.DeveloperFeatureId = cc.DeveloperFeatureId
				Join CodeFeatures cf on cf.CodeFeatureKey = df.CodeFeatureKey
			where cc.CodeCommitMetricsKey = NEW.CodeCommitMetricsKey
        ) THEN
			SIGNAL SQLSTATE '45000'
			SET MESSAGE_TEXT = 'Code commit data must be less that feature code complete date';
        END IF;
	END;

DELIMITER ;
