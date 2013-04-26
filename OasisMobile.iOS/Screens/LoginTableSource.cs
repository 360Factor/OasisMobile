using System;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;
using BigTed;
using System.Threading.Tasks;
using System.Net;

namespace OasisMobile.iOS
{
	public class LoginTableSource : UITableViewSource
	{
		public LoginTableSource ()
		{
		}

		private UITextField txtUserName;
		private UITextField txtPassword;
		private UIImageView imgLogo;
		private UIButton btnLogin;
	
		#region implemented abstract members of UITableViewSource
		
		public override int RowsInSection (UITableView tableview, int section)
		{

			switch (section) {
			case 0:
				return 1;
			case 1:
				return 2;
			case 2:
				return 1;
			default:
				return 0;

			}

		}
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{

			UITableViewCell cell;
			
			switch (indexPath.Section) {
			case 0:
				cell = tableView.DequeueReusableCell ("logoCell");
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, "logoCell");
				}
				UIImage loginLogoImage = new UIImage ("Images/OasisLogo560px.png");
				imgLogo = new UIImageView (loginLogoImage);
				imgLogo.Frame = new System.Drawing.RectangleF (AppDelegate.window.Frame.Width / 2 - 140, 10, 280, 90); 
				cell.ContentView.AddSubview (imgLogo);
				return cell;
			case 1:
				cell = tableView.DequeueReusableCell ("cell");
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
					cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				}
				if (indexPath.Row == 0) {
					cell.TextLabel.Text = "Username";
					txtUserName = new UITextField (new System.Drawing.RectangleF (110, 10, 185, 30));
					txtUserName.KeyboardType = UIKeyboardType.Default;
					txtUserName.Placeholder = "Enter your Username";
					txtUserName.ShouldReturn += delegate {
						txtUserName.ResignFirstResponder ();
						return true;
					};
					cell.ContentView.AddSubview (txtUserName);
				} else if (indexPath.Row == 1) {
					
					cell.TextLabel.Text = "Password";
					txtPassword = new UITextField (new System.Drawing.RectangleF (110, 10, 185, 30));
					txtPassword.KeyboardType = UIKeyboardType.Default;
					txtPassword.SecureTextEntry = true;
					txtPassword.Placeholder = "Enter your Password";
					txtPassword.ShouldReturn += delegate {
						txtPassword.ResignFirstResponder ();
						return true;
					};
					cell.ContentView.AddSubview (txtPassword);
				}
				return cell;
			case 2:
				cell = new UITableViewCell ();
		
				btnLogin = new UIButton (UIButtonType.RoundedRect);
				btnLogin.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
				btnLogin.Frame = new System.Drawing.RectangleF (0, 0, cell.Frame.Width, 36);
				btnLogin.SetTitle ("Login", UIControlState.Normal);
				btnLogin.TouchUpInside += btnLogin_Clicked;
				cell.ContentView.AddSubview (btnLogin);
				return cell;
			default:
				cell = tableView.DequeueReusableCell ("cell");
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");

				}
				return cell;
			}



		}
		
		#endregion
		
		public override int NumberOfSections (UITableView tableView)
		{
			return 3; //3 sections for login page. 1 for logo view, 1 for credential, 1 for button
			// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
		}

		public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			// NOTE: Don't call the base implementation on a Model class
			// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
			if ((indexPath.Section == 0 || indexPath.Section == 2) && indexPath.Row == 0) {
				cell.BackgroundView = null;
			}
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			// NOTE: Don't call the base implementation on a Model class
			// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
			if (indexPath.Section == 1) {
				tableView.DeselectRow (indexPath, false);
			}

		}

		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			// NOTE: Don't call the base implementation on a Model class
			// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
			if (indexPath.Section == 0) {
				return 100; 
			} else {
				return 44;
			}

		}

		private void btnLogin_Clicked (object sender, EventArgs e)
		{
//			bool loginSuccessful = false;
//			WebClient service = new WebClient ();
//			service.Headers.Add (HttpRequestHeader.Accept, "application/json");
//			string serviceURL = AppConfig.BaseWebserviceURL +
//				"UserByLoginNameAndPassword/" + 
//					System.Web.HttpUtility.UrlEncode (txtUserName.Text) + "/" + 
//					System.Web.HttpUtility.UrlEncode (txtPassword.Text);
//			string responseString = service.DownloadString (serviceURL);
//			
//			if (responseString != null && responseString != "") {
//				var _jsonUser = System.Json.JsonValue.Parse (responseString);
//				BussinessLogicLayer.User _loggedInUser;
//				_loggedInUser = BussinessLogicLayer.User.GetUserByMainSystemID (_jsonUser ["UserID"]);
//				if (_loggedInUser == null) {
//					_loggedInUser = new BussinessLogicLayer.User ();
//					_loggedInUser.MainSystemID = _jsonUser ["UserID"];
//				}
//				_loggedInUser.LoginName = _jsonUser ["LoginName"];
//				_loggedInUser.UserName = _jsonUser ["UserName"];
//				_loggedInUser.Password = _jsonUser ["Password"];
//				_loggedInUser.EmailAddress = _jsonUser ["UserEmail"];
//				_loggedInUser.LastLoginDate = DateTime.Now;
//				_loggedInUser.Save ();
//				loginSuccessful = true;
//				AppSession.LoggedInUser = _loggedInUser;
//			} else {
//				loginSuccessful = false;
//			}

		

			if (txtUserName.Text != "" && txtPassword.Text != "") {
				BTProgressHUD.Show ("Please wait");
				bool loginSuccessful = false;
				string userName = txtUserName.Text;
				string password = txtPassword.Text;
				Task.Factory.StartNew (() => {
					WebClient service = new WebClient ();
					service.Headers.Add (HttpRequestHeader.Accept, "application/json");
					string serviceURL = AppConfig.BaseWebserviceURL +
										"UserByLoginNameAndPassword/" + 
										System.Web.HttpUtility.UrlEncode (userName) + "/" + 
										System.Web.HttpUtility.UrlEncode (password);
					string responseString = service.DownloadString (serviceURL);
					if (responseString != null && responseString != "") {
						var _jsonUser = System.Json.JsonValue.Parse (responseString);
						BussinessLogicLayer.User _loggedInUser;
						_loggedInUser = BussinessLogicLayer.User.GetUserByMainSystemID (_jsonUser ["UserID"]);
						if (_loggedInUser == null) {
							_loggedInUser = new BussinessLogicLayer.User ();
							_loggedInUser.MainSystemID = _jsonUser ["UserID"];
						}
						_loggedInUser.LoginName = _jsonUser ["LoginName"];
						_loggedInUser.UserName = _jsonUser ["UserName"];
						_loggedInUser.Password = _jsonUser ["Password"];
						_loggedInUser.EmailAddress = _jsonUser ["UserEmail"];
						_loggedInUser.LastLoginDate = DateTime.Now;
						_loggedInUser.Save ();
						loginSuccessful = true;
						AppSession.LoggedInUser = _loggedInUser;
						SyncManager.SyncExamDataFromServer();
						SyncManager.SyncUserExamDataFromServer (AppSession.LoggedInUser);

					} else {
						loginSuccessful = false;
					}
					
				}).ContinueWith (task1 => {
					if (loginSuccessful) {
						//After login, we download the exam list and the user's exam before displaying the exam
						BTProgressHUD.Dismiss ();
						AppDelegate.m_loginViewController.DismissViewController (true, null);
					} else {
						BTProgressHUD.Dismiss ();
						UIAlertView _invalidCredentialAlert = new UIAlertView ("Invalid Credential", "The credential you entered is invalid, please try again", null, "Ok", null);
						_invalidCredentialAlert.Show ();
					}
				}, TaskScheduler.FromCurrentSynchronizationContext ());
				
			} else {
				UIAlertView _requiredFieldAlert = new UIAlertView ("Credential Required", "Please enter your credential in the provided field", null, "Ok", null);
				_requiredFieldAlert.Show ();
			}
		}
	}
}

