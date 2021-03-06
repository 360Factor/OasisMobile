using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BigTed;
using System.Threading.Tasks;

namespace OasisMobile.iOS
{
	public partial class SettingsView : FlyoutNavigationBaseViewController
	{
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public SettingsView ()
			: base (UserInterfaceIdiomIsPhone ? "SettingsView_iPhone" : "SettingsView_iPad", null)
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
			this.Title = "Settings";
			
			// Perform any additional setup after loading the view, typically from a nib.
			tblvSettings.Source = new SettingsViewTableSource (this);
		}

		public class SettingsViewTableSource : UITableViewSource
		{
			//This class has the following section and rows:
			//1.1 LoggedInUserInfo
			//2.1 KeepMeLoggedIn
			//3.1 AutoAdvanceQuestion
			//3.2 AutoSubmitQuestion
			//4.1 SyncData
			//5.1 Logout

			UIButton btnLogout;
			UIButton btnSyncExam;
			UISwitch swPersistentLogin;
			UISwitch swAutoAdvanceQuestion;
			UISwitch swAutoSubmitResponse;

			public enum SettingsViewSections
			{
				LoggedInUserInfo = 0
,
				PersistentLoginOption = 1
,
				QuestionSubmisionOptions = 2
,
				SyncDataButton = 3
,
				LogoutButton = 4
			}

			private UIViewController m_currentViewController = null;

			public SettingsViewTableSource (UIViewController ParentViewController)
			{
				m_currentViewController = ParentViewController;

			}
			#region implemented abstract members of UITableViewSource

			public override int RowsInSection (UITableView tableview, int section)
			{
				switch (section) {
				case (int)SettingsViewSections.LoggedInUserInfo:
					return 1;
				case (int)SettingsViewSections.PersistentLoginOption:
					return 1;
				case (int)SettingsViewSections.QuestionSubmisionOptions:
					return 2;
				case (int)SettingsViewSections.SyncDataButton:
					return 1;
				case (int)SettingsViewSections.LogoutButton:
					return 1;
				default:
					return 0;
				}

			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{

				UITableViewCell cell;

				switch (indexPath.Section) {
				case (int)SettingsViewSections.LoggedInUserInfo:
					cell = tableView.DequeueReusableCell ("userInfoCell");
					if (cell == null) {
						cell = new UITableViewCell (UITableViewCellStyle.Default, "userInfoCell");
					}
					cell.TextLabel.Font = UIFont.SystemFontOfSize (24);
					cell.ImageView.Image = UIImage.FromBundle ("Images/Icon-User.png");
					cell.TextLabel.Text = AppSession.LoggedInUser.UserName;
					cell.TextLabel.BackgroundColor = UIColor.Clear;

					return cell;
				case (int)SettingsViewSections.PersistentLoginOption:
					cell = tableView.DequeueReusableCell ("cell");
					if (cell == null) {
						cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
					}
					cell.TextLabel.Text = "Keep Me Logged In";
					if (swPersistentLogin == null) {
						swPersistentLogin = new UISwitch ();
						swPersistentLogin.ValueChanged += swPersistentLogin_ValueChanged;
						swPersistentLogin.On = AppSettings.PersistentLogin;
					} 
					cell.AccessoryView = swPersistentLogin;
					return cell;
				case (int)SettingsViewSections.QuestionSubmisionOptions:
					cell = tableView.DequeueReusableCell ("cell");
					if (cell == null) {
						cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
					}
					if (indexPath.Row == 0) {
						cell.TextLabel.Text = "Auto Advance Question";
						if (swAutoAdvanceQuestion == null) {
							swAutoAdvanceQuestion = new UISwitch ();
							swAutoAdvanceQuestion.ValueChanged += swAutoAdvanceQuestion_ValueChanged;
							swAutoAdvanceQuestion.On = AppSettings.AutoAdvanceQuestion;
						} 
						cell.AccessoryView = swAutoAdvanceQuestion;
					} else if (indexPath.Row == 1) {
						cell.TextLabel.Text = "Auto Submit Response";
						if (swAutoSubmitResponse == null) {
							swAutoSubmitResponse = new UISwitch ();
							swAutoSubmitResponse.ValueChanged += swAutoSubmitResponse_ValueChanged;
							swAutoSubmitResponse.On = AppSettings.AutoSubmitResponse;
						} 
						cell.AccessoryView = swAutoSubmitResponse;
					}
					return cell;
				case (int)SettingsViewSections.SyncDataButton:
					cell = new UITableViewCell (UITableViewCellStyle.Default, null);
					btnSyncExam = new UIButton (UIButtonType.RoundedRect);
					btnSyncExam.Frame = cell.ContentView.Bounds;
					btnSyncExam.AutoresizingMask = UIViewAutoresizing.All;
					btnSyncExam.SetTitle ("Sync Data", UIControlState.Normal);
					btnSyncExam.TouchUpInside += btnSyncExam_Clicked;
					cell.ContentView.AddSubview (btnSyncExam);
					return cell;

				case (int)SettingsViewSections.LogoutButton:
					cell = new UITableViewCell (UITableViewCellStyle.Default, null);
					btnLogout = new UIButton (UIButtonType.RoundedRect);
					btnLogout.Frame = cell.ContentView.Bounds;
					btnLogout.AutoresizingMask = UIViewAutoresizing.All;
					btnLogout.SetTitle ("Logout", UIControlState.Normal);
					btnLogout.TouchUpInside += btnLogout_Clicked;
					cell.ContentView.AddSubview (btnLogout);


					return cell;
				}
			
				return null;
			}
			#endregion

			public override int NumberOfSections (UITableView tableView)
			{
				return Enum.GetValues (typeof(SettingsViewSections)).Length;
				// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
			}

			public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				if (indexPath.Section == (int)SettingsViewSections.LoggedInUserInfo || 
				    indexPath.Section == (int)SettingsViewSections.SyncDataButton ||
				    indexPath.Section == (int)SettingsViewSections.LogoutButton) {
					cell.BackgroundView = null;
					cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				}

			}

			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				if (indexPath.Section == (int)SettingsViewSections.LoggedInUserInfo) {
					return 52;
				} else {
					return 44;
				}
			}

			private void swPersistentLogin_ValueChanged (object sender, EventArgs e)
			{
				if (swPersistentLogin.On) {
					AppSettings.PersistentLogin = true;
					AppSettings.LoggedInLoginName = AppSession.LoggedInUser.LoginName;
				} else {
					AppSettings.PersistentLogin = false;
					AppSettings.LoggedInLoginName = null;
				}
			}

			private void swAutoAdvanceQuestion_ValueChanged (object sender, EventArgs e)
			{
				AppSettings.AutoAdvanceQuestion = swAutoAdvanceQuestion.On;
			}

			private void swAutoSubmitResponse_ValueChanged (object sender, EventArgs e)
			{
				AppSettings.AutoSubmitResponse = swAutoSubmitResponse.On;
			}

			private void btnLogout_Clicked (object sender, EventArgs e)
			{
				AppSettings.LoggedInLoginName = "";

				AppDelegate.m_flyoutMenuController.SelectedIndex = 0;
				AppDelegate.m_flyoutMenuController.ExamTab.PopToRootViewController (false);

				LoginView _loginViewController = new LoginView ();
				_loginViewController.ModalInPopover = false;
				_loginViewController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;

				AppDelegate.m_flyoutMenuController.PresentViewController (_loginViewController, true, null);
				//TODO: clear session here without crashing the views
			
			}

			private void btnSyncExam_Clicked (object sender, EventArgs e)
			{
				AppSession.SelectedExam = null;
//				AppDelegate.m_flyoutMenuController.ExamTab.PopToRootViewController (false);
				AppDelegate.m_flyoutMenuController.ExamTab.SetViewControllers (new UIViewController[]{new ExamListView()}, false);

				BTProgressHUD.Show ("Syncing");
				bool _syncSuccessful = true;
				Task.Factory.StartNew (()=>{
					try{
						if(SyncManager.PushAllDoSyncData()){
							SyncManager.SyncExamDataFromServer();
							SyncManager.SyncUserExamDataFromServer (AppSession.LoggedInUser);
							SyncManager.SyncUserExamAccess (AppSession.LoggedInUser);
							SyncManager.SyncUserQuestionAndAnswerFromServer(AppSession.LoggedInUser,true);
						}else{
							_syncSuccessful = false;
						}
					
					}
					catch(Exception ex){
						Console.WriteLine (ex.ToString ());
						_syncSuccessful=false;
					}

			}).ContinueWith (task1 =>{
					BTProgressHUD.Dismiss ();
					if(!_syncSuccessful){
						UIAlertView _unsuccessfulSyncAlert = new UIAlertView("Sync Failed",
						                                                     "The sync process cannot be completed. Please try again later", 
						                                                     null,"Ok",null);
						_unsuccessfulSyncAlert.Show ();
					}
				
				},TaskScheduler.FromCurrentSynchronizationContext ());
			
			}
		}
	}
}

