using System;

namespace OasisMobile.iOS
{
	public static class AppSession
	{
		public static BusinessModel.User LoggedInUser {get; set;}

		public static BusinessModel.Exam SelectedExam {get; set;}

		public static BusinessModel.UserExam SelectedUserExam {get; set;}


	}
}

