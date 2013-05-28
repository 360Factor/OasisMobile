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
		public UINavigationController ExamTab{
			get{
				return m_examTab;
			}
		}
		private UINavigationController m_examTab;
		private UINavigationController m_settingsTab;
		private UINavigationController m_supportTab;
		private UINavigationController m_aboutTab;
		public OasisFlyoutController () : base()
		{
			m_examTab = new UINavigationController ();
			m_examTab.PushViewController (new ExamListView (), false);
			m_settingsTab = new UINavigationController ();
			m_settingsTab.PushViewController (new SettingsView (), false);
			m_supportTab = new UINavigationController ();
			m_supportTab.PushViewController (new SupportView (), false);
			m_aboutTab = new UINavigationController ();
			m_aboutTab.PushViewController (new AboutView (), false);


			// Create the navigation menu
			NavigationRoot = new RootElement ("Navigation") {
				new Section () {
					new StyledStringElement ("My Exams")      
					{ 
						BackgroundColor = UIColor.Clear, 
						TextColor = UIColor.DarkGray, 
						Image = new UIImage(UIImage.FromBundle ("Images/Icon-Book.png").CGImage,2,UIImageOrientation.Up)
					},
					new StyledStringElement ("Settings")    
					{ 
						BackgroundColor = UIColor.Clear, 
						TextColor = UIColor.DarkGray,
						Image = new UIImage(UIImage.FromBundle ("Images/Icon-Gear.png").CGImage,2,UIImageOrientation.Up)
					},
					new StyledStringElement ("Support")
					{ 
						BackgroundColor = UIColor.Clear, 
						TextColor = UIColor.DarkGray,
						Image = new UIImage(UIImage.FromBundle ("Images/Icon-Lifebuoy.png").CGImage,2,UIImageOrientation.Up)
					},
					new StyledStringElement ("About")
					{ 
						BackgroundColor = UIColor.Clear, 
						TextColor = UIColor.DarkGray,
						Image = new UIImage(UIImage.FromBundle ("Images/Icon-Beaker.png").CGImage,2,UIImageOrientation.Up)
					}
				}
			};

			ViewControllers = new UIViewController[] {
				m_examTab, m_settingsTab, m_supportTab, m_aboutTab
			};

			NavigationTableView.BackgroundView = new UIImageView (UIImage.FromBundle ("Images/Background-Paper.jpg"));
			NavigationTableView.SeparatorColor = UIColor.DarkGray;

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if ((this.InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft ||
				this.InterfaceOrientation == UIInterfaceOrientation.LandscapeRight) && !this.IsOpen) {
				ShowMenu ();
			} else if ((this.InterfaceOrientation == UIInterfaceOrientation.Portrait || 
			           this.InterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown) && this.IsOpen) {
				HideMenu ();
			}
		}

	}

}