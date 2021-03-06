using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Linq;
using System.Collections.Generic;

namespace OasisMobile.iOS
{
	
	public class QuestionSplitView : UISplitViewController
	{
		private Question_iPad m_questionView;
		private ExamQuestionList_iPad m_examQuestionListView;
		public QuestionSplitView () : base()
		{
			if (AppSession.SelectedExamUserQuestionList == null) {
				AppSession.SelectedExamUserQuestionList = BusinessModel.UserQuestion.GetUserQuestionsBySQL (string.Format (
					"SELECT * FROM tblUserQuestion " +
					"WHERE fkUserExamID={0} ORDER BY Sequence", AppSession.SelectedUserExam.UserExamID));
			}

			m_examQuestionListView = new ExamQuestionList_iPad (0);
			m_examQuestionListView.QuestionSelected += QuestionSelected;
			m_questionView = new Question_iPad (AppSession.SelectedExamUserQuestionList[0]);
			m_questionView.QuestionUpdated+= QuestionUpdated;
			m_questionView.ViewedQuestionChanged+= QuestionView_ViewedQuestionChanged;
			ViewControllers = new UIViewController[] {
				m_examQuestionListView, m_questionView
			};

			this.ShouldHideViewController = delegate(UISplitViewController svc, UIViewController viewController, UIInterfaceOrientation inOrientation) {
				return inOrientation == UIInterfaceOrientation.Portrait
				|| inOrientation == UIInterfaceOrientation.PortraitUpsideDown;
			};

			this.WillHideViewController+= (object sender, UISplitViewHideEventArgs e) => {
				m_questionView.AddQuestionListButton (e.BarButtonItem);
				m_questionView.Popover = e.Pc;
			};

			this.WillShowViewController += (object sender, UISplitViewShowEventArgs e) => {
				m_questionView.RemoveQuesitonListButton ();
				m_questionView.Popover = null;
			};

		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();


			this.WillRotate (this.InterfaceOrientation, 0);
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

		private void QuestionSelected(object sender, ExamQuestionList_iPad.QuestionSelectedEventArgs e){
			if (m_questionView != null) {
				m_questionView.DisplayUserQuestion (e.SelectedQuestion);
			}

		}

		private void QuestionUpdated(object sender, EventArgs e){
			m_examQuestionListView.ReloadQuestionListTableView ();
		}

		private void QuestionView_ViewedQuestionChanged(object sender, Question_iPad.ViewedQuestionChangedEventArgs e){
			m_examQuestionListView.MarkQuestionAsSelected (e.CurrentUserQuestionIndex);
		}

		public void PresentAsRootViewWithAnimation(){
			UIViewAnimationOptions _transitionStyle = UIViewAnimationOptions.TransitionFlipFromRight;
			switch (AppDelegate.window.RootViewController.InterfaceOrientation) {
			case UIInterfaceOrientation.LandscapeLeft:
				_transitionStyle = UIViewAnimationOptions.TransitionFlipFromBottom;
				break;
			case UIInterfaceOrientation.LandscapeRight:
				_transitionStyle = UIViewAnimationOptions.TransitionFlipFromTop;
				break;
			case UIInterfaceOrientation.Portrait:
				_transitionStyle = UIViewAnimationOptions.TransitionFlipFromRight;
				break;
			case UIInterfaceOrientation.PortraitUpsideDown:
				_transitionStyle = UIViewAnimationOptions.TransitionFlipFromLeft;
				break;
			}
			AppDelegate.window.RootViewController = this;
			UIView.Transition (AppDelegate.window,
			                   0.5,
			                   _transitionStyle,
			                   () => {
				AppDelegate.window.RootViewController = this;
				this.WillAnimateRotation (AppDelegate.m_flyoutMenuController.InterfaceOrientation, 0);
			}, null);
		}

	}
}

