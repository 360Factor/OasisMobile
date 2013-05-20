using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

using FlyoutNavigation;

namespace OasisMobile.iOS
{
	public class QuestionFlyoutSplitView : FlyoutNavigationController
	{
		private Question_iPad m_questionView;
		private ExamQuestionList_iPad m_examQuestionListView;
		public QuestionFlyoutSplitView () : base()
		{
			if (AppSession.SelectedExamUserQuestionList == null) {
				AppSession.SelectedExamUserQuestionList = BusinessModel.UserQuestion.GetUserQuestionsBySQL (string.Format (
					"SELECT * FROM tblUserQuestion " +
					"WHERE fkUserExamID={0} ORDER BY Sequence", AppSession.SelectedUserExam.UserExamID));
			}

			m_examQuestionListView = new ExamQuestionList_iPad (0);
			m_questionView = new Question_iPad (AppSession.SelectedExamUserQuestionList[0]);
			ViewControllers = new UIViewController[] {
				m_examQuestionListView, m_questionView
			};
	
		}


		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (AppSession.SelectedExamUserQuestionList == null) {
				AppSession.SelectedExamUserQuestionList = BusinessModel.UserQuestion.GetUserQuestionsBySQL (string.Format (
					"SELECT * FROM tblUserQuestion " +
					"WHERE fkUserExamID={0} ORDER BY Sequence", AppSession.SelectedUserExam.UserExamID));
			}
		}



	}


}