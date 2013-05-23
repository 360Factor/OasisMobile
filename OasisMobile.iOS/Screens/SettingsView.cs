using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

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
			//4.1 Logout

			UIButton btnLogout;
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
				LogoutButton = 3
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
					cell.TextLabel.Font = UIFont.SystemFontOfSize (26);
					cell.ImageView.Image = UIImage.FromBundle ("Images/Icon-User.png");
					cell.TextLabel.Text = AppSession.LoggedInUser.LoginName;
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
						swPersistentLogin.On = NSUserDefaults.StandardUserDefaults.BoolForKey ("PersistentLogin");
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
							swAutoAdvanceQuestion.On = NSUserDefaults.StandardUserDefaults.BoolForKey ("AutoAdvanceQuestion");
						} 
						cell.AccessoryView = swAutoAdvanceQuestion;
					} else if (indexPath.Row == 1) {
						cell.TextLabel.Text = "Auto Submit Response";
						if (swAutoSubmitResponse == null) {
							swAutoSubmitResponse = new UISwitch ();
							swAutoSubmitResponse.ValueChanged += swAutoSubmitResponse_ValueChanged;
							swAutoSubmitResponse.On = NSUserDefaults.StandardUserDefaults.BoolForKey ("AutoSubmitResponse");
						} 
						cell.AccessoryView = swAutoSubmitResponse;
					}
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
				return 4;
				// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
			}

			public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				if (indexPath.Section == (int)SettingsViewSections.LoggedInUserInfo || indexPath.Section == (int)SettingsViewSections.LogoutButton) {
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

			private void swPersistentLogin_ValueChanged (object sender, EventArgs e){
				NSUserDefaults.StandardUserDefaults.SetBool (swPersistentLogin.On,"PersistentLogin");
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			}

			private void swAutoAdvanceQuestion_ValueChanged (object sender, EventArgs e){
				NSUserDefaults.StandardUserDefaults.SetBool (swAutoAdvanceQuestion.On,"AutoAdvanceQuestion");
				NSUserDefaults.StandardUserDefaults.Synchronize ();

			}

			private void swAutoSubmitResponse_ValueChanged (object sender, EventArgs e){
				NSUserDefaults.StandardUserDefaults.SetBool (swAutoSubmitResponse.On,"AutoSubmitResponse");
				NSUserDefaults.StandardUserDefaults.Synchronize ();

			}

			private void btnLogout_Clicked (object sender, EventArgs e)
			{
				AppSession.ClearSession ();
				m_currentViewController.NavigationController.PopToRootViewController (false);
				AppDelegate.m_flyoutMenuController.SelectedIndex = 0;
				LoginView _loginViewController = new LoginView ();
				_loginViewController.ModalInPopover = false;
				_loginViewController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;



				AppDelegate.m_flyoutMenuController.PresentViewController (_loginViewController,true, null);
			}
		}
	}
}

