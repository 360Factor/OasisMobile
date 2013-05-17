
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

			UIImage loginImage = new UIImage("Images/OasisBG.png");
			UIImageView loginBgView = new UIImageView(loginImage);
			loginBgView.ContentMode=UIViewContentMode.ScaleAspectFill;
			tblvLogin.BackgroundView = loginBgView;
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
			tblvLogin.Source = new LoginTableSource();
		}


	}
}

