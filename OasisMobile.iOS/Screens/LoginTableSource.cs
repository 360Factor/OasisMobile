using System;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using BigTed;
using System.Threading.Tasks;
using System.Net;
using System.Drawing;

namespace OasisMobile.iOS
{
	public class LoginTableSource : UITableViewSource
	{
		private UIViewController m_currentViewController;
		public LoginTableSource (UIViewController aCurrentViewController)
		{
			m_currentViewController = aCurrentViewController;

		}

		private UITextField txtUserName;
		private UITextField txtPassword;
		private UIButton btnLogin;
		private UIButton btnForgotPassword;
		private UIButton btnRegister;
		#region implemented abstract members of UITableViewSource
		
		public override int RowsInSection (UITableView tableview, int section)
		{

			switch (section) {
			case 0: //Spacer
				return 1;
			case 1: //UserName & password
				return 2;
			case 2: //LoginButton
				return 1;
			case 3: //ForgotPassword & Register Links
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
//				cell = tableView.DequeueReusableCell ("logoCell");
//				if (cell == null) {
//					cell = new CustomImageCell ("logoCell");
////					imgLogo = new UIImageView (loginLogoImage);
////					imgLogo.Frame = new System.Drawing.RectangleF (tableView.Frame.Width/ 2 - 140, 10, 280, 90); 
//
////					cell.ContentView.AddSubview (imgLogo);
//				}
//				UIImage loginLogoImage = new UIImage (AppConfig.ImagePaths.ClientLogo);
//				((CustomImageCell)cell).MaxImageDimension = loginLogoImage.Size.Width / 2; //We use retina image
//				cell.ImageView.Image = loginLogoImage;

				cell = tableView.DequeueReusableCell ("spacerCell");
				if(cell == null){
					cell = new UITableViewCell (UITableViewCellStyle.Default, "spacerCell");
				}
				break;
			case 1:
//				cell = tableView.DequeueReusableCell ("cell");
//				if (cell == null) {
//					cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
//					cell.AutoresizingMask = UIViewAutoresizing.None;
//					RectangleF _cellFrame = cell.Frame;
//					_cellFrame.X = (_cellFrame.X + (_cellFrame.X + _cellFrame.Width)) / 2 - 100;
//					_cellFrame.Width = 200;
//					cell.Frame = _cellFrame;
//				}
//
				cell = tableView.DequeueReusableCell ("inputCell");
				if (cell == null) {
					cell = new CustomTextFieldCell ( "inputCell");
					cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				}

				var inputCell = (CustomTextFieldCell)cell;
				if(UIDevice.CurrentDevice.UserInterfaceIdiom== UIUserInterfaceIdiom.Pad){
					inputCell.InputTextWidthPct = 75;
				}

				if (indexPath.Row == 0) {

					inputCell.TextLabel.Text = "Username";

					if (txtUserName == null) {
						txtUserName = inputCell.InputTextField;
					}
					else{
						string _userName = txtUserName.Text;
						txtUserName = inputCell.InputTextField;
						txtUserName.Text = _userName;
					}
					txtUserName.KeyboardType = UIKeyboardType.Default;
					txtUserName.Placeholder = "Enter your Username";
					txtUserName.AutocapitalizationType = UITextAutocapitalizationType.None;
					txtUserName.AutocorrectionType = UITextAutocorrectionType.No;
					txtUserName.ShouldReturn += delegate {
						txtUserName.ResignFirstResponder ();
						txtPassword.BecomeFirstResponder ();
						return true;
					};

//					txtUserName = new UITextField (new System.Drawing.RectangleF (110, 10, 185, 30));


//					foreach (UIView _subview in cell.ContentView.Subviews) {
//						_subview.RemoveFromSuperview ();
//					}
//					cell.ContentView.AddSubview (txtUserName);
				} else if (indexPath.Row == 1) {
					
					inputCell.TextLabel.Text = "Password";
					if (txtPassword == null) {
						txtPassword = inputCell.InputTextField;
				
					} else {
						string _password = txtPassword.Text;
						txtPassword = inputCell.InputTextField;
						txtPassword.Text = _password;
					}

					txtPassword.KeyboardType = UIKeyboardType.Default;
					txtPassword.SecureTextEntry = true;
					txtPassword.Placeholder = "Enter your Password";
					txtPassword.ShouldReturn += delegate {
						txtPassword.ResignFirstResponder ();
						ProcessLogin ();
						return true;
					};
//					txtPassword = new UITextField (new System.Drawing.RectangleF (110, 10, 185, 30));
//					cell.ContentView.AddSubview (txtPassword);
				}
				break;
			case 2:
				cell = tableView.DequeueReusableCell ("buttonCell");
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, "buttonCell");
				}
		
				btnLogin = new UIButton (UIButtonType.RoundedRect);
				btnLogin.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
				btnLogin.Frame = new System.Drawing.RectangleF (0, 0, cell.ContentView.Frame.Width, 36);
				btnLogin.SetTitle ("Login", UIControlState.Normal);
				btnLogin.TouchUpInside += btnLogin_Clicked;

				foreach (UIButton _button in cell.ContentView.Subviews) {
					_button.RemoveFromSuperview ();
				}
				cell.ContentView.AddSubview (btnLogin);
				break;
			case 3:
				cell = tableView.DequeueReusableCell ("linksCell");
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, "linksCell");
				}

				foreach (UIButton _button in cell.ContentView.Subviews) {
					_button.RemoveFromSuperview ();
				}

				btnForgotPassword = new UIButton (UIButtonType.Custom);
				btnForgotPassword.SetTitle ("Forgot Password", UIControlState.Normal);
				btnForgotPassword.TitleLabel.AdjustsFontSizeToFitWidth = true;
				btnForgotPassword.Frame = new System.Drawing.RectangleF (new PointF(0,0), 
				                                                          tableView.StringSize (btnForgotPassword.Title (UIControlState.Normal), UIFont.SystemFontOfSize (14)));
				btnForgotPassword.TouchUpInside += (object sender, EventArgs e) => {
					UIApplication.SharedApplication.OpenUrl (NSUrl.FromString ( "http://selfassessment.sportsmed.org/Public/PasswordRecovery.aspx"));
				};

				cell.ContentView.AddSubview (btnForgotPassword);


				btnRegister = new UIButton (UIButtonType.Custom);
				btnRegister.SetTitle ("Register", UIControlState.Normal);
				btnRegister.TitleLabel.AdjustsFontSizeToFitWidth = true;
				if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
					btnRegister.Frame = new System.Drawing.RectangleF (new PointF(btnForgotPassword.Frame.Width + 30,0), 
					                                                   tableView.StringSize (btnRegister.Title (UIControlState.Normal),UIFont.SystemFontOfSize (14)));
				}else{
					btnRegister.Frame = new System.Drawing.RectangleF (new PointF(0,btnForgotPassword.Frame.Height), 
					                                                   tableView.StringSize (btnRegister.Title (UIControlState.Normal),UIFont.SystemFontOfSize (14)));
				}

					btnRegister.TouchUpInside += (object sender, EventArgs e) => {
						UIApplication.SharedApplication.OpenUrl (NSUrl.FromString ("http://www.sportsmed.org"));
					};

		
					cell.ContentView.AddSubview (btnRegister);
		


				break;
			default:
				cell = tableView.DequeueReusableCell ("cell");
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");

				}
				break;
			}
			return cell;


		}
		#endregion
		
		public override int NumberOfSections (UITableView tableView)
		{
			return 4; //4 sections for login page. 1 for logo view, 1 for credential, 1 for button, and one for the links
			// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
		}

		public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			// NOTE: Don't call the base implementation on a Model class
			// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
			if ((indexPath.Section == 0 || indexPath.Section == 2) && indexPath.Row == 0) {
				cell.BackgroundView = null;
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			}else if(indexPath.Section ==3){
				cell.BackgroundView = null;
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			}


		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			// NOTE: Don't call the base implementation on a Model class
			// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
			if (indexPath.Section == 0 || indexPath.Section == 1 || indexPath.Section==3) {
				tableView.DeselectRow (indexPath, false);
			}

		}

		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			// NOTE: Don't call the base implementation on a Model class
			// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
			if (indexPath.Section == 0) {
//				return 100;
				if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
					return 200;
				} else {
					return 75;
				}
			} else if (indexPath.Section == 3) {
				if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
					return tableView.StringSize ("Test",UIFont.SystemFontOfSize (14)).Height;
				} else {
					return 2 * tableView.StringSize ("Test",UIFont.SystemFontOfSize (14)).Height + 20;
				}
			
			} else {
				return 44;
			}

		}

		private void btnLogin_Clicked (object sender, EventArgs e)
		{
			txtUserName.ResignFirstResponder ();
			txtPassword.ResignFirstResponder ();
			btnLogin.ResignFirstResponder ();
			ProcessLogin ();
		}

		private void ProcessLogin ()
		{
			if (txtUserName.Text != null && txtPassword.Text != null &&
				txtUserName.Text != "" && txtPassword.Text != "") {
				//Clear the session just in case there is leftover session
				AppSession.ClearSession ();
				if (Reachability.InternetConnectionStatus () == NetworkStatus.NotReachable) {
					//Not reachable, tell user to try again later
					UIAlertView _alert = new UIAlertView ("No Connection", "Please connect to the internet to login", null, "Ok", null);
					_alert.Show ();
					return;
				}

				//If network is reachable, login
				BTProgressHUD.Show ("Please wait");
				bool _loginSuccessful = false;
				bool _serviceReachable = true;
				string userName = txtUserName.Text;
				string password = txtPassword.Text;
			
				Task.Factory.StartNew (() => {
					string responseString ="";
					try{
						WebClient service = new WebClient ();
						service.Headers.Add (HttpRequestHeader.Accept, "application/json");
						string serviceURL = AppConfig.BaseWebserviceURL +
							"UserByLoginNameAndPassword/" + 
								System.Web.HttpUtility.UrlEncode (userName) + "/" + 
								System.Web.HttpUtility.UrlEncode (password);
						responseString = service.DownloadString (serviceURL);

						if (responseString != null && responseString != "") {
							var _jsonUser = System.Json.JsonValue.Parse (responseString);
							BusinessModel.User _loggedInUser;
							_loggedInUser = BusinessModel.User.GetUserByMainSystemID (_jsonUser ["UserID"]);
							if (_loggedInUser == null) {
								_loggedInUser = new BusinessModel.User ();
								_loggedInUser.MainSystemID = _jsonUser ["UserID"];
							}
							_loggedInUser.LoginName = _jsonUser ["LoginName"];
							_loggedInUser.UserName = _jsonUser ["UserName"];
							_loggedInUser.Password = _jsonUser ["Password"];
							_loggedInUser.EmailAddress = _jsonUser ["UserEmail"];
							_loggedInUser.LastLoginDate = DateTime.Now;
							_loggedInUser.Save ();
							_loginSuccessful = true;
							AppSession.LoggedInUser = _loggedInUser;
							SyncManager.PushAllDoSyncData();
							int _examCount = BusinessModel.SQL.ExecuteScalar<int> ("SELECT COUNT(*) FROM tblExam", new object[]{});
							if(_examCount==0){
								//Only sync if there is not exam for now
								try{
									SyncManager.SyncExamDataFromServer();
								}catch(Exception ex){
									Console.WriteLine ("Exception on synching exam data from server");
									Console.WriteLine (ex.ToString());
									throw ex;
								}


							}

							try{
								SyncManager.SyncUserExamDataFromServer (AppSession.LoggedInUser);
							}catch(Exception ex){
								Console.WriteLine ("Exception on synching user exam data from server");
								Console.WriteLine (ex.ToString());
								throw ex;
							}

							try{
								SyncManager.SyncUserExamAccess (AppSession.LoggedInUser);
							}catch(Exception ex){
								Console.WriteLine ("Exception on synching user exam access data from server");
								Console.WriteLine (ex.ToString());
								throw ex;
							}

							
						} else {
							_loginSuccessful = false;
						}

					}
					catch(Exception ex){
						Console.WriteLine ("Unable to login as the service encountered an error for username: " +userName);
						Console.WriteLine (ex.ToString ());
						_loginSuccessful = false;
						_serviceReachable = false;
					}
	
			
				}).ContinueWith (task1 => {
					if (_loginSuccessful) {
						//After login, we download the exam list and the user's exam before displaying the exam
						BTProgressHUD.Dismiss ();
						m_currentViewController.DismissViewController (true, null);
						AppDelegate.m_flyoutMenuController.WillAnimateRotation (m_currentViewController.InterfaceOrientation, 0);
						if(AppSettings.PersistentLogin){
							//If the settings are in persistent login, we will put the just logged in person login into the settings
							AppSettings.LoggedInLoginName = AppSession.LoggedInUser.LoginName;
						}
					} else {
						BTProgressHUD.Dismiss ();
						if(_serviceReachable){
							UIAlertView _invalidCredentialAlert = new UIAlertView ("Invalid Credential", "The credential you entered is invalid, please try again", null, "Ok", null);
							_invalidCredentialAlert.Show ();
						}else{
							UIAlertView _alert = new UIAlertView ("Service Unavailable", "We could not contact login server to verify your credential. Please try again later", null, "Ok", null);
							_alert.Show ();
						}
					
					}
				}, TaskScheduler.FromCurrentSynchronizationContext ());
				
			} else {
				UIAlertView _requiredFieldAlert = new UIAlertView ("Credential Required", "Please enter your credential in the provided field", null, "Ok", null);
				_requiredFieldAlert.Show ();
			}
		}
	}
}

