
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
		}

		public class ExamQuestionsTableSource : UITableViewSource
		{
			private List<BusinessModel.UserQuestion> m_examQuestionListTableData;

			private UIViewController m_currentViewController=null;
			
			public ExamQuestionsTableSource (UIViewController ParentViewController)
			{
				m_currentViewController = ParentViewController;
				m_examQuestionListTableData = BusinessModel.UserQuestion.GetUserQuestionsBySQL(string.Format (
					"SELECT * FROM UserQuestion " +
					"WHERE fkUserExamID={0} ORDER BY Sequence", AppSession.SelectedUserExam.UserExamID));
			}
			
			#region implemented abstract members of UITableViewSource
			
			public override int RowsInSection (UITableView tableview, int section)
			{
				return m_examQuestionListTableData.Count;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				
				UITableViewCell cell;
				
				cell = tableView.DequeueReusableCell ("cell");
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
				}
				
				BusinessModel.UserQuestion _userQuestion = m_examQuestionListTableData[indexPath.Row];
				
				cell.TextLabel.Text = "Question " + _userQuestion.Sequence;
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				
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

				tableView.DeselectRow(indexPath,false);
			}
			
			
		}

	}
}

