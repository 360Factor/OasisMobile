using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.Dialog;

// TODO: DemoFlyout: subclass the component to keep the configuration in one place
using FlyoutNavigation;

namespace OasisMobile.iOS
{
	public class OasisFlyoutController : FlyoutNavigationController
	{
		public OasisFlyoutController () : base()
		{
			var vc1 = new UINavigationController ();
			vc1.PushViewController (new SessionsViewController (), false);
			var vc2 = new UINavigationController ();
			vc2.PushViewController (new SpeakersViewController(), false);
			var vc3 = new UINavigationController();
			vc3.PushViewController (new AboutViewController(), false);
			
			
			// Create the navigation menu
			NavigationRoot = new RootElement ("Navigation") {
				new Section () {
					new StyledStringElement ("Exams")    { BackgroundColor = UIColor.Clear, TextColor = UIColor.LightGray },
					new StyledStringElement ("Account")    { BackgroundColor = UIColor.Clear, TextColor = UIColor.LightGray },
					new StyledStringElement ("About Oasis"){ BackgroundColor = UIColor.Clear, TextColor = UIColor.LightGray },
				}
			};
			
			// Supply view controllers corresponding to menu items:
			ViewControllers = new UIViewController[] {
				vc1, vc2, vc3
			};
			
			NavigationTableView.BackgroundView = new UIImageView (UIImage.FromBundle ("images/Background-Party.png"));
			NavigationTableView.SeparatorColor = UIColor.DarkGray;
		}
	}
}