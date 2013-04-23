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

		private int m_rowCount = 2;
		private UITextField txtUserName;
		private UITextField txtPassword;
		
		#region implemented abstract members of UITableViewSource
		
		public override int RowsInSection (UITableView tableview, int section)
		{

			switch (section) {
			case 0:
				break;
			case 1:
				return 1;
			case 2:
				return 2;
			default:
				return 0;

			}

			return  m_rowCount;
		}
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{

			var cell = tableView.DequeueReusableCell ("cell");
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
			}
			
			switch (indexPath.Section) {
			case 0:
		

			case 1:
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
			case 2:

			default:
				break;
			}


			


			return cell;
		}
		
		#endregion
		
		public override int NumberOfSections (UITableView tableView)
		{
			return 3; //3 sections for login page. 1 for logo view, 1 for credential, 1 for button
			// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
		}

	}
}

