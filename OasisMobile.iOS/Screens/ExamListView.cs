
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Linq;
using System.Collections.Generic;

namespace OasisMobile.iOS
{
	public partial class ExamListView : FlyoutNavigationBaseViewController
	{
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public ExamListView ()
			: base (UserInterfaceIdiomIsPhone ? "ExamListView_iPhone" : "ExamListView_iPad", null)
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
			this.Title = "Exams";
			// Perform any additional setup after loading the view, typically from a nib.


		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
//			if(tblvExamList.Source==null){
//				tblvExamList.Source = new ExamListTableSource();
//			}

		}

	}

	public class ExamListTableSource : UITableViewSource
	{
		private List<BussinessLogicLayer.Exam> m_examList;
		private List<BussinessLogicLayer.UserExam> m_userExamList;
		private List<BussinessLogicLayer.Exam> m_startedExamList;
		private List<BussinessLogicLayer.Exam> m_newExamList;

		public ExamListTableSource ()
		{
			m_examList = BussinessLogicLayer.Exam.GetAllExams ();
			m_userExamList = BussinessLogicLayer.UserExam.GetUserExamsByUserID (AppSession.LoggedInUser.UserID);
		
			List<int> _startedExamIDList = (from x in m_userExamList select x.ExamID).ToList ();
			List<int> _newExamIDList = (from x in m_examList 
			                            where x.IsExpired && !_startedExamIDList.Contains (x.ExamID) 
			                            select x.ExamID).ToList(); 
			m_startedExamList = (from x in m_examList where _startedExamIDList.Contains (x.ExamID) select x).ToList();
			m_newExamList = (from x in m_examList where _newExamIDList.Contains(x.ExamID) select x).ToList();
		}
		
		#region implemented abstract members of UITableViewSource
		
		public override int RowsInSection (UITableView tableview, int section)
		{
			
			switch (section) {
			case 0:
				return m_startedExamList.Count;
			case 1:
				return m_newExamList.Count;
			default:
				return 0;
				
			}
			
		}
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			
			UITableViewCell cell;

			cell = tableView.DequeueReusableCell ("cell");
			if(cell==null){
				cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
			}

//			switch (indexPath.Section) {
//			case 0:
//				cell.TextLabel.Text = m_startedExamList[indexPath.Row].ExamName;
//			case 1:
//				cell.TextLabel.Text = m_newExamList[indexPath.Row].ExamName;
//			default:
//				cell.TextLabel.Text = "";
//			}
			return cell;
		}
		
		#endregion
		
		public override int NumberOfSections (UITableView tableView)
		{
			return 2; //2 sections for exam list page. 1 for started, 1 for new
			// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
		}

		public override string TitleForHeader (UITableView tableView, int section)
		{
			// NOTE: Don't call the base implementation on a Model class
			// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
			switch (section) {
			case 0:
				return "Started";
			case 1:
				return "New";
			default:
				return "";
			}
		}
		
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			// NOTE: Don't call the base implementation on a Model class
			// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
			
		}


	}

}

