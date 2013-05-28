using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BigTed;

namespace OasisMobile.iOS
{
	public partial class LoginView : UIViewController
	{
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public LoginView ()
			: base (UserInterfaceIdiomIsPhone ? "LoginView_iPhone" : "LoginView_iPad", null)
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
	
			UIImage loginImage;

			if (UserInterfaceIdiomIsPhone) {
				if (AppDelegate.window.Frame.Height == 568) {
					//for iphone 5 and above
					loginImage = new UIImage (AppConfig.ImagePaths.iPhone.LoginBackgroundImage_568h);
				} else {
					loginImage = new UIImage (AppConfig.ImagePaths.iPhone.LoginBackgroundImage);
				}

			} else {
				loginImage = new UIImage (AppConfig.ImagePaths.iPad.LoginBackgroundImage);
			}
			UIImageView loginBgView = new UIImageView (loginImage);
			loginBgView.ContentMode = UIViewContentMode.ScaleAspectFill;
			tblvLogin.BackgroundView = loginBgView;
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
			tblvLogin.Source = new LoginTableSource (this);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			UIApplication.SharedApplication.SetStatusBarStyle (UIStatusBarStyle.BlackOpaque, animated);

		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			UIApplication.SharedApplication.SetStatusBarStyle (UIStatusBarStyle.Default, animated);

		}
	}
}

