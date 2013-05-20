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
		private UINavigationController m_examTab;
		private UINavigationController m_accountTab;
		private UINavigationController m_aboutTab;
		public OasisFlyoutController () : base()
		{
			m_examTab = new UINavigationController ();
			m_examTab.PushViewController (new ExamListView (), false);
			m_accountTab = new UINavigationController ();
			m_accountTab.PushViewController (new AccountView (), false);
			m_aboutTab = new UINavigationController ();
			m_aboutTab.PushViewController (new AboutView (), false);

			// Create the navigation menu
			NavigationRoot = new RootElement ("Navigation") {
				new Section () {
					new StyledStringElement ("My Exams")      
					{ 
						BackgroundColor = UIColor.Clear, 
						TextColor = UIColor.DarkGray, 
						Image = UIImage.FromBundle ("Images/Icon-Book.png")
					},
					new StyledStringElement ("Settings")    
					{ 
						BackgroundColor = UIColor.Clear, 
						TextColor = UIColor.DarkGray,
						Image = UIImage.FromBundle ("Images/Icon-User.png") 
					},
					new StyledStringElement ("Support")
					{ 
						BackgroundColor = UIColor.Clear, 
						TextColor = UIColor.DarkGray,
						Image = UIImage.FromBundle ("Images/Icon-Experiment.png")
					},
					new StyledStringElement ("About")
					{ 
						BackgroundColor = UIColor.Clear, 
						TextColor = UIColor.DarkGray,
						Image = UIImage.FromBundle ("Images/Icon-Experiment.png")
					}
				}
			};

			ViewControllers = new UIViewController[] {
				m_examTab, m_accountTab, m_aboutTab
			};

			NavigationTableView.BackgroundView = new UIImageView (UIImage.FromBundle ("Images/Background-Paper.jpg"));
			NavigationTableView.SeparatorColor = UIColor.DarkGray;

		}

	}

}