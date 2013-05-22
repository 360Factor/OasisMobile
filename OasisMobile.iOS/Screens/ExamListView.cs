
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
			this.Title = "My Exams";
			// Perform any additional setup after loading the view, typically from a nib.

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (AppSession.LoggedInUser != null) {
				if (tblvExamList.Source == null) {
					tblvExamList.Source = new ExamListTableSource (this);
				}else{
					tblvExamList.Source = new ExamListTableSource (this);
					tblvExamList.ReloadData ();
				}
				AppSession.SelectedExam = null;
				AppSession.SelectedUserExam = null;

			}
		}


		public class ExamListTableSource : UITableViewSource
		{
			private class ExamListData
			{
				public int ExamID { get; set; }
				
				public string ExamName { get; set; }
				
				public string ExamStatus { get; set; }
				
				public ExamListData ()
				{
				}
				
				
			}		

			private Dictionary<string, List<ExamListData>> m_userExamTableViewData = new Dictionary<string, List<ExamListData>> ();

			private UIViewController m_currentViewController=null;

			public ExamListTableSource (UIViewController ParentViewController)
			{
				m_currentViewController = ParentViewController;
				string _examPurchaseFilter = "";
				if (!AppConfig.AllowExamPurchase) {
					_examPurchaseFilter = " AND tblExamAccess.HasAccess = 1";
				}
				
				List<Tuple<string,string,string>> _tempExamTuple = BusinessModel.SQL.Get3Tuples (string.Format (
					"SELECT tblExam.pkExamID, tblExam.ExamName, " +
					"(CASE WHEN tblUserExam.pkUserExamID IS NULL THEN 'New' WHEN tblUserExam.IsSubmitted=1 THEN 'Completed' ELSE 'Started' END) " +
					"FROM tblExam LEFT JOIN tblExamAccess " +
					"ON tblExam.pkExamID = tblExamAccess.fkExamID AND tblExamAccess.fkUserID={0} LEFT JOIN tblUserExam " +
					"ON tblExam.pkExamID = tblUserExam.fkExamID AND tblUserExam.fkUserID={0} " +
					"WHERE (tblExamAccess.pkExamAccessID IS NOT NULL {1}) OR tblUserExam.pkUserExamID IS NOT NULL " +
					"ORDER BY tblExam.ExamName DESC", AppSession.LoggedInUser.UserID, _examPurchaseFilter));
				
				List<ExamListData> _userExamList = 
					(from x in _tempExamTuple select new ExamListData (){
						ExamID = Convert.ToInt32 (x.Item1),
						ExamName = x.Item2,
						ExamStatus = x.Item3}).ToList ();
				
				foreach (ExamListData _examData in _userExamList) {
					if (m_userExamTableViewData.ContainsKey (_examData.ExamStatus)) {
						//If the dictionary already contains the key, that means there is already another item in the list with the same exam status, so we just add the examData to the list referenced with the key
						m_userExamTableViewData [_examData.ExamStatus].Add (_examData);
					} else {
						//Otherwise, there is no exam that is referenced by the key, that means we add the key to dictionary and create a new list of ExamListData with the exam we are adding as the first member
						m_userExamTableViewData.Add (_examData.ExamStatus, new List<ExamListData>{_examData});
					}
				}
			}
			
			#region implemented abstract members of UITableViewSource
			
			public override int RowsInSection (UITableView tableview, int section)
			{
				return m_userExamTableViewData.ElementAt (section).Value.Count;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				
				UITableViewCell cell;
				
				cell = tableView.DequeueReusableCell ("cell");
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
				}
				
				ExamListData _cellExamData = m_userExamTableViewData.ElementAt (indexPath.Section).Value [indexPath.Row];
				
				cell.TextLabel.Text = _cellExamData.ExamName;
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				
				return cell;
			}
			
			#endregion
			
			public override int NumberOfSections (UITableView tableView)
			{
				return m_userExamTableViewData.Count;
				// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
			}
			
			public override string TitleForHeader (UITableView tableView, int section)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				return m_userExamTableViewData.ElementAt (section).Key;
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 

				ExamListData _cellExamData = m_userExamTableViewData.ElementAt (indexPath.Section).Value [indexPath.Row];
				m_currentViewController.NavigationController.PushViewController (new ExamDetailView(_cellExamData.ExamID),true);
				tableView.DeselectRow(indexPath,false);
			}

			
			
		}


	}


}

