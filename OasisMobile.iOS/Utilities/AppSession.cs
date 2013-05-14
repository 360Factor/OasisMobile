using System;
using System.Collections.Generic;
namespace OasisMobile.iOS
{
	public static class AppSession
	{
		public static BusinessModel.User LoggedInUser {get; set;}

		public static BusinessModel.Exam SelectedExam {get; set;}

		public static BusinessModel.UserExam SelectedUserExam {get; set;}

		public static List<BusinessModel.UserQuestion> SelectedExamUserQuestionList {get; set;}

		public static BusinessModel.UserQuestion CurrentDisplayedUserQuestion {get; set;}

		public static BusinessModel.UserQuestion NextUserQuestion {get; set;}

		public static BusinessModel.UserQuestion PreviousUserQuestion {get; set;}

		//public static DateTime QuestionViewStartTimeStamp{ get; set; }
	}
}

