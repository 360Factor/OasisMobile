
using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace OasisMobile.iOS
{
	public class FlyoutNavigationBaseViewController : UIViewController
	{
		public FlyoutNavigationBaseViewController ()
		{
		}

		public FlyoutNavigationBaseViewController (string nibName, NSBundle bundle) : base(nibName,bundle)
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || 
				InterfaceOrientation == UIInterfaceOrientation.LandscapeRight) {
				NavigationItem.SetLeftBarButtonItem (null, false);
			} else {
				var bbi = new UIBarButtonItem(UIImage.FromBundle ("Images/Slideout.png"), UIBarButtonItemStyle.Plain, (sender, e) => {
					AppDelegate.m_flyoutMenuController.ToggleMenu();
				});
				NavigationItem.SetLeftBarButtonItem (bbi, false);
			}

		}

		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillRotate (toInterfaceOrientation, duration);
			if (toInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || 
				toInterfaceOrientation == UIInterfaceOrientation.LandscapeRight) {
				NavigationItem.SetLeftBarButtonItem (null, true);
			} else {
				var bbi = new UIBarButtonItem(UIImage.FromBundle ("Images/Slideout.png"), UIBarButtonItemStyle.Plain, (sender, e) => {
					AppDelegate.m_flyoutMenuController.ToggleMenu();
				});
				NavigationItem.SetLeftBarButtonItem (bbi, true);
			}
		}

	
	}


}
