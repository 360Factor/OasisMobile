
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Linq;
using System.Collections.Generic;

namespace OasisMobile.iOS
{
	public partial class ExamQuestionList_iPhone : UIViewController
	{
		public ExamQuestionList_iPhone () : base ("ExamQuestionList_iPhone", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			this.Title = "Questions";
			AppSession.SelectedExamUserQuestionList = BusinessModel.UserQuestion.GetUserQuestionsBySQL (string.Format (
				"SELECT * FROM UserQuestion " +
				"WHERE fkUserExamID={0} ORDER BY Sequence", AppSession.SelectedUserExam.UserExamID));
			UIViewController[] _originalNavigationStack = 	NavigationController.ViewControllers;
			List<UIViewController> _updatedNavigationStack = new List<UIViewController>();

			foreach(UIViewController _viewController in _originalNavigationStack){
				//Eliminate the 
				if(_viewController.GetType () != typeof(GenerateNewExamView)){
					_updatedNavigationStack.Add (_viewController);
				}
			}
			NavigationController.ViewControllers = _updatedNavigationStack.ToArray();

		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (tblvExamQuestionList.Source != null) {
				tblvExamQuestionList.Source = new ExamQuestionsTableSource (this);
				tblvExamQuestionList.ReloadData ();
			} else {
				tblvExamQuestionList.Source = new ExamQuestionsTableSource (this);
			}
			AppSession.CurrentDisplayedUserQuestion = null;
			AppSession.NextUserQuestion = null;
			AppSession.PreviousUserQuestion = null;

		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
		}

		public class ExamQuestionsTableSource : UITableViewSource
		{
			private UIViewController m_currentViewController = null;
			
			public ExamQuestionsTableSource (UIViewController ParentViewController)
			{
				m_currentViewController = ParentViewController;

			}
			
			#region implemented abstract members of UITableViewSource
			
			public override int RowsInSection (UITableView tableview, int section)
			{
				return AppSession.SelectedExamUserQuestionList.Count;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				
				UITableViewCell cell;
				
				cell = tableView.DequeueReusableCell ("cell");
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
				}
				
				BusinessModel.UserQuestion _userQuestion = AppSession.SelectedExamUserQuestionList [indexPath.Row];
				
				cell.TextLabel.Text = "Question " + _userQuestion.Sequence;
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				if (_userQuestion.HasAnswered) {
					if (AppSession.SelectedUserExam.IsSubmitted || AppSession.SelectedUserExam.IsLearningMode) {
						//If exam is submitted or the exam is in learning mode, we show whether the answer is correct or not
						if (_userQuestion.HasAnsweredCorrectly) {
							cell.ImageView.Image = UIImage.FromBundle("Images/Icon-Yes.png");
							cell.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
						} else {
							cell.ImageView.Image = UIImage.FromBundle("Images/Icon-No.png");
							cell.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
						}
					} else {
						//Otherwise, we just show the fact that the question has been answered (by removing the not answered mark)
						cell.ImageView.Image = UIImage.FromBundle("Images/Transparent.png").Scale (new SizeF(32,32));
						cell.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
					}
				}
				else{
					cell.ImageView.Image = UIImage.FromBundle ("Images/Icon-QuestionMark.png");
					cell.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				}

				return cell;
			}
			
			#endregion
			
			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
				// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 

				UIViewController _questionView = new Question_iPhone( AppSession.SelectedExamUserQuestionList[indexPath.Row]);
				m_currentViewController.NavigationController.PushViewController (_questionView,true);
				tableView.DeselectRow (indexPath, false);
			}
			
			
		}

	}
}

