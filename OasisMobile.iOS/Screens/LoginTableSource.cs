using System;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace OasisMobile.iOS
{
	public class LoginTableSource : UITableViewSource
	{
		public LoginTableSource ()
		{
		}

		public class LoginData
		{
			string UserName { get; set; }

			string password { get; set; }
			
			public LoginData ()
			{
				
			}
			
		}
		
		LoginData Credential { get; set; }

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
				UIImageView imgLogo = new UIImageView (loginLogoImage);
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
				btnLogin.TouchUpInside += (object sender, EventArgs e) => {
					if (txtUserName.Text == "john.doe" && txtPassword.Text == "password") {

						//AppDelegate.m_navController.SetViewControllers (new UIViewController[]{new ExamListView()},true);
						//AppDelegate.m_navController.PushViewController (new ExamListView (), true);
						AppDelegate.m_loginViewController.DismissViewController (true,null);
					} else {
						UIAlertView _invalidCredentialAlert = new UIAlertView ("Invalid Credential", "You have entered an invalid credential", null, "Ok", null);
						_invalidCredentialAlert.Show ();
					}

				};
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
	}
}

