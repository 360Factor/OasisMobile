
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

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
			
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			this.NavigationController.SetNavigationBarHidden (false, animated);
			UIBarButtonItem btnFlyoutMenu = new UIBarButtonItem(UIBarButtonSystemItem.PageCurl, delegate (object sender, EventArgs e) {
				Console.WriteLine ("button clicked");
			});

			this.NavigationController.NavigationBar.TopItem.LeftBarButtonItem = btnFlyoutMenu;
		
		}


		void ShowFlyoutMenu (MonoTouch.Foundation.NSObject sender)
		{
			AppDelegate.m_flyoutMenuController.ShowMenu ();
		}

	}
}

