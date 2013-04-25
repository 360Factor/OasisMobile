
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
			
			var bbi = new UIBarButtonItem(UIImage.FromBundle ("Images/Slideout.png"), UIBarButtonItemStyle.Plain, (sender, e) => {
				AppDelegate.m_flyoutMenuController.ToggleMenu();
			});
			NavigationItem.SetLeftBarButtonItem (bbi, false);
		}
	}


}
