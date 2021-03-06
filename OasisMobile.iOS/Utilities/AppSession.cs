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

		//public static DateTime QuestionViewStartTimeStamp{ get; set; }

		public static void ClearSession(){
			LoggedInUser = null;
			SelectedExam = null;
			SelectedUserExam = null;
			SelectedExamUserQuestionList = null;
		}
	}
}

