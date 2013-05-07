

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
        this.Execute("CREATE TABLE IF NOT EXISTS [Category] ( " +
			"  [pkCategoryID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [CategoryName] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [DisplayOrder] INTEGER NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [ParentCategoryID] INTEGER,  " +
			"  [ExpandedCategoryName] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_Category_MainSystemID] ON [Category] ([MainSystemID]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [ExamType] ( " +
			"  [pkExamTypeID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [ExamTypeName] TEXT NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_ExamType_ExamTypeName] ON [ExamType] ([ExamTypeName]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [Exam] ( " +
			"  [pkExamID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [ExamName] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [fkExamTypeID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_Exam_ExamType_ExamTypeID] REFERENCES [ExamType]([pkExamTypeID]),  " +
			"  [IsExpired] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [Credit] INTEGER NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [Price] REAL NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [MinimumPassingScore] REAL NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [Disclosure] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [PrivacyPolicy] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [Description] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_Exam_ExamTypeID] ON [Exam] ([fkExamTypeID]); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_Exam_MainSystemID] ON [Exam] ([MainSystemID]); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_ExamName] ON [Exam] ([ExamName]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [Question] ( " +
			"  [pkQuestionID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [Stem] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [LeadIn] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [Commentary] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [Reference] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [fkCategoryID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_CategoryID_Category_Question] REFERENCES [Category]([pkCategoryID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [fkExamID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_ExamID_Exam_Question] REFERENCES [Exam]([pkExamID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [PopulationCorrectPct] FLOAT,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_Question_CategoryID] ON [Question] ([fkCategoryID]); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_Question_MainSystemID] ON [Question] ([MainSystemID]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [AnswerOption] ( " +
			"  [pkAnswerOptionID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [fkQuestionID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_QuestionID_Question_AnswerOption] REFERENCES [Question]([pkQuestionID]),  " +
			"  [AnswerText] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [IsCorrect] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_AnswerOption_MainSystemID] ON [AnswerOption] ([MainSystemID]); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_AnswerOption_QuestionID] ON [AnswerOption] ([fkQuestionID]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [User] ( " +
			"  [pkUserID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [LoginName] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [Password] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [EmailAddress] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [UserName] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [LastLoginDate] DATETIME NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_EmailAddress] ON [User] ([EmailAddress]); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_LoginName] ON [User] ([LoginName]); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_User_MainSystemID] ON [User] ([MainSystemID]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [ExamAccess] ( " +
			"  [pkExamAccessID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [fkUserID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_UserID_Exam_User] REFERENCES [User]([pkUserID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [fkExamID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_ExamID_Exam_ExamAccess] REFERENCES [Exam]([pkExamID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [HasAccess] BOOLEAN NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_ExamAccess_UserID] ON [ExamAccess] ([fkUserID]); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_UserAccess_ExamID] ON [ExamAccess] ([fkExamID]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [Image] ( " +
			"  [pkImageID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [fkQuestionID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_QuestionID_Image_Question] REFERENCES [Question]([pkQuestionID]),  " +
			"  [Title] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [ShowInQuestion] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [ShowInCommentary] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [FilePath] TEXT NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [DownloadURL] TEXT,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_Image_MainSystemID] ON [Image] ([MainSystemID]); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_Image_QuestionID] ON [Image] ([fkQuestionID]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [UserQuestion] ( " +
			"  [pkUserQuestionID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [fkQuestionID] INTEGER NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [fkUserExamID] INTEGER NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [Sequence] INTEGER NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [AnsweredDateTime] DATETIME,  " +
			"  [HasAnswered] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [HasAnsweredCorrectly] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [SecondsSpent] INTEGER NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [DoSync] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_UserQuestion_MainSystemID] ON [UserQuestion] ([MainSystemID]); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_UserQuestion_QuestionID] ON [UserQuestion] ([fkQuestionID]); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_UserQuestion_UserExamID] ON [UserQuestion] ([fkUserExamID]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [UserAnswerOption] ( " +
			"  [pkUserAnswerOptionID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [fkUserQuestionID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_UserID_UserAnswerOption_UserQuestion] REFERENCES [UserQuestion]([pkUserQuestionID]),  " +
			"  [fkAnswerOptionID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_AnswerOptionID_UserAnswerOption_AnswerOption] REFERENCES [AnswerOption]([pkAnswerOptionID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [Sequence] INTEGER NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [IsSelected] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_UserAnswerOption_AnswerOptionID] ON [UserAnswerOption] ([fkUserQuestionID]); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_UserAnswerOption_MainSystemID] ON [UserAnswerOption] ([MainSystemID]); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_UserAnswerOption_UserQuestionID] ON [UserAnswerOption] ([fkUserQuestionID]); " );

        this.Execute("CREATE TABLE IF NOT EXISTS [UserExam] ( " +
			"  [pkUserExamID] INTEGER NOT NULL ON CONFLICT ROLLBACK PRIMARY KEY ON CONFLICT ROLLBACK AUTOINCREMENT,  " +
			"  [fkUserID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_UserID_User_UserExam] REFERENCES [User]([pkUserID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [fkExamID] INTEGER NOT NULL ON CONFLICT ROLLBACK CONSTRAINT [FK_ExamID_Exam_UserExam] REFERENCES [Exam]([pkExamID]) ON DELETE CASCADE ON UPDATE CASCADE,  " +
			"  [IsCompleted] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [IsSubmitted] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [IsLearningMode] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [HasReadDisclosure] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [HasReadPrivacyPolicy] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [SecondsSpent] INTEGER NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [IsDownloaded] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [DoSync] BOOLEAN NOT NULL ON CONFLICT ROLLBACK,  " +
			"  [MainSystemID] INTEGER NOT NULL ON CONFLICT ROLLBACK); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_UserExam_ExamID] ON [UserExam] ([fkExamID]); " );

        this.Execute("CREATE UNIQUE INDEX IF NOT EXISTS [IX_UserExam_MainSystemID] ON [UserExam] ([MainSystemID]); " );

        this.Execute("CREATE INDEX IF NOT EXISTS [IX_UserExam_UserID] ON [UserExam] ([fkUserID]); " );

        }
    }

}

