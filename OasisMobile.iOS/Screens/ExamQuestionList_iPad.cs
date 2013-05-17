using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Linq;
using System.Collections.Generic;

namespace OasisMobile.iOS
{
	public partial class ExamQuestionList_iPad : UIViewController
	{
		private UITableView tblvExamQuestionList;
		private int m_selectedRowIndex = -1;

		public int CurrentSelectedRowIndex {
			get {
				return m_selectedRowIndex;
			}set {
				m_selectedRowIndex = value;
			}
		}

		public event EventHandler<QuestionSelectedEventArgs> QuestionSelected;

		public ExamQuestionList_iPad (int aRowToPreselect) : base ()
		{
			m_selectedRowIndex = aRowToPreselect;
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

			tblvExamQuestionList = new UITableView (this.View.Bounds);
			tblvExamQuestionList.AutoresizingMask = UIViewAutoresizing.All;
			tblvExamQuestionList.Source = new ExamQuestionList_iPadTableSource (this);
			View.AddSubview (tblvExamQuestionList);

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (tblvExamQuestionList.Source != null) {
				tblvExamQuestionList.Source = new ExamQuestionList_iPadTableSource (this);
				tblvExamQuestionList.ReloadData ();
			}
			tblvExamQuestionList.SelectRow (NSIndexPath.FromRowSection (m_selectedRowIndex,0), true,UITableViewScrollPosition.None);
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
		}

		public void ReloadQuestionListTableView ()
		{
			tblvExamQuestionList.Source = new ExamQuestionList_iPadTableSource (this);
			tblvExamQuestionList.ReloadData ();
			tblvExamQuestionList.SelectRow (NSIndexPath.FromRowSection (m_selectedRowIndex,0), true,UITableViewScrollPosition.None);
		}

		public void MarkQuestionAsSelected (int aRowIndex)
		{
			tblvExamQuestionList.SelectRow (NSIndexPath.FromRowSection (aRowIndex,0), true, UITableViewScrollPosition.Middle);
			m_selectedRowIndex = aRowIndex;
		}

		public class QuestionSelectedEventArgs : EventArgs
		{

			public BusinessModel.UserQuestion SelectedQuestion{ get; set; }

			public QuestionSelectedEventArgs (BusinessModel.UserQuestion aSelectedQuestion):base()
			{
				SelectedQuestion = aSelectedQuestion;
			}
		}

		public class ExamQuestionList_iPadTableSource : UITableViewSource
		{
			private ExamQuestionList_iPad m_currentViewController = null;

			public ExamQuestionList_iPadTableSource (ExamQuestionList_iPad ParentViewController)
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
							cell.ImageView.Image = UIImage.FromBundle ("Images/Icon-Yes.png").Scale (new SizeF(24,24));
							cell.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
						} else {
							cell.ImageView.Image = UIImage.FromBundle ("Images/Icon-No.png").Scale (new SizeF(24,24));
							cell.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
						}
					} else {
						//Otherwise, we just show the fact that the question has been answered (by removing the not answered mark)
						cell.ImageView.Image = UIImage.FromBundle ("Images/Transparent.png").Scale (new SizeF(24,24));
						cell.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
					}
				} else {
					cell.ImageView.Image = UIImage.FromBundle ("Images/Icon-QuestionMark.png").Scale (new SizeF(24,24));
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

				m_currentViewController.QuestionSelected (tableView, new QuestionSelectedEventArgs (AppSession.SelectedExamUserQuestionList[indexPath.Row]));
				m_currentViewController.CurrentSelectedRowIndex = indexPath.Row;
			}
		}
	}
}

