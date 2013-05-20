

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile.BusinessModel
{

    public class Repository : SQLiteConnection
    {
    	public static object Locker = new object ();

    	public static Repository Instance { get { return _instance; } }

    	private static readonly Repository _instance = new Repository (ConnectionString.DBPath);

    	private bool _isInitialized = false;

    	private Repository (string DBPath) : base(DBPath)
    	{
    	}

		public void InitializeDb(){
			if(_isInitialized){
				throw new Exception("The database is already initialized, the initializeDb method should not be called twice");
			}else{
				CreateDatabase();
				Constant.ExamType.SeedEnumTable();
				_isInitialized = true;
			}

		}

        private void CreateDatabase()
        {
        this.Execute("CREATE TABLE IF NOT EXISTS [tblCategory] ( " +
			"  [pkCategoryID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [CategoryName] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [DisplayOrder] INTEGER NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [ParentCategoryID] INTEGER,  " +
			"  [ExpandedCategoryName] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_Category_MainSystemID] ON [tblCategory] ([MainSystemID]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [tblExamType] ( " +
			"  [pkExamTypeID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [ExamTypeName] TEXT NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_ExamType_ExamTypeName] ON [tblExamType] ([ExamTypeName]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [tblExam] ( " +
			"  [pkExamID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [ExamName] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [fkExamTypeID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_Exam_ExamType_ExamTypeID] REFERENCES [tblExamType]([pkExamTypeID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [IsExpired] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [Credit] INTEGER NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [Price] REAL NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [MinimumPassingScore] REAL NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [Disclosure] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [PrivacyPolicy] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [Description] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_Exam_ExamTypeID] ON [tblExam] ([fkExamTypeID]); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_Exam_MainSystemID] ON [tblExam] ([MainSystemID]); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_ExamName] ON [tblExam] ([ExamName]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [tblQuestion] ( " +
			"  [pkQuestionID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [Stem] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [LeadIn] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [Commentary] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [Reference] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [fkCategoryID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_CategoryID_Category_Question] REFERENCES [tblCategory]([pkCategoryID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [fkExamID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_ExamID_Exam_Question] REFERENCES [tblExam]([pkExamID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [PopulationCorrectPct] FLOAT,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_Question_CategoryID] ON [tblQuestion] ([fkCategoryID]); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_Question_MainSystemID] ON [tblQuestion] ([MainSystemID]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [tblAnswerOption] ( " +
			"  [pkAnswerOptionID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [fkQuestionID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_QuestionID_Question_AnswerOption] REFERENCES [tblQuestion]([pkQuestionID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [AnswerText] TEXT NOT NULL ON CONFLICT ROLLBACK COLLATE NOCASE,  " +
			"  [IsCorrect] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_AnswerOption_MainSystemID] ON [tblAnswerOption] ([MainSystemID]); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_AnswerOption_QuestionID] ON [tblAnswerOption] ([fkQuestionID]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [tblUser] ( " +
			"  [pkUserID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [LoginName] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [Password] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [EmailAddress] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [UserName] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [LastLoginDate] DATETIME NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_EmailAddress] ON [tblUser] ([EmailAddress]); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_LoginName] ON [tblUser] ([LoginName]); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_User_MainSystemID] ON [tblUser] ([MainSystemID]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [tblExamAccess] ( " +
			"  [pkExamAccessID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [fkUserID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_UserID_Exam_User] REFERENCES [tblUser]([pkUserID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [fkExamID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_ExamID_Exam_ExamAccess] REFERENCES [tblExam]([pkExamID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [HasAccess] BOOLEAN NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_ExamAccess_UserID] ON [tblExamAccess] ([fkUserID]); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_UserAccess_ExamID] ON [tblExamAccess] ([fkExamID]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [tblImage] ( " +
			"  [pkImageID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [fkQuestionID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_QuestionID_Image_Question] REFERENCES [tblQuestion]([pkQuestionID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [Title] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [ShowInQuestion] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [ShowInCommentary] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [FilePath] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [DownloadURL] TEXT,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_Image_MainSystemID] ON [tblImage] ([MainSystemID]); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_Image_QuestionID] ON [tblImage] ([fkQuestionID]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [tblUserQuestion] ( " +
			"  [pkUserQuestionID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [fkQuestionID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_QuestionID_UserQuestion_Question] REFERENCES [tblQuestion]([pkQuestionID]) ON DELETE CASCADE ON UPDATE CASCADE,    " +
			"  [fkUserExamID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_UserExamID_UserQuestion_UserExam] REFERENCES [tblUserExam]([pkUserExamID]) ON DELETE CASCADE ON UPDATE CASCADE,    " +
			"  [Sequence] INTEGER NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [AnsweredDateTime] DATETIME,  " +
			"  [HasAnswered] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [HasAnsweredCorrectly] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [SecondsSpent] INTEGER NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [DoSync] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_UserQuestion_MainSystemID] ON [tblUserQuestion] ([MainSystemID]); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_UserQuestion_QuestionID] ON [tblUserQuestion] ([fkQuestionID]); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_UserQuestion_UserExamID] ON [tblUserQuestion] ([fkUserExamID]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [tblUserAnswerOption] ( " +
			"  [pkUserAnswerOptionID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [fkUserQuestionID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_UserID_UserAnswerOption_UserQuestion] REFERENCES [tblUserQuestion]([pkUserQuestionID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [fkAnswerOptionID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_AnswerOptionID_UserAnswerOption_AnswerOption] REFERENCES [tblAnswerOption]([pkAnswerOptionID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [Sequence] INTEGER NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [IsSelected] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_UserAnswerOption_AnswerOptionID] ON [tblUserAnswerOption] ([fkUserQuestionID]); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_UserAnswerOption_MainSystemID] ON [tblUserAnswerOption] ([MainSystemID]); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_UserAnswerOption_UserQuestionID] ON [tblUserAnswerOption] ([fkUserQuestionID]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [tblUserExam] ( " +
			"  [pkUserExamID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [fkUserID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_UserID_User_UserExam] REFERENCES [tblUser]([pkUserID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [fkExamID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_ExamID_Exam_UserExam] REFERENCES [tblExam]([pkExamID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [IsCompleted] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [IsSubmitted] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [IsLearningMode] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [HasReadDisclosure] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [HasReadPrivacyPolicy] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [SecondsSpent] INTEGER NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [IsDownloaded] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [DoSync] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_UserExam_ExamID] ON [tblUserExam] ([fkExamID]); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_UserExam_MainSystemID] ON [tblUserExam] ([MainSystemID]); " );

        }
    }

}

