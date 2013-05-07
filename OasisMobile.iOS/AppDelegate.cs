using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace OasisMobile.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		public static UIWindow window;
		//public static UINavigationController m_navController;
		public static UIViewController m_loginViewController;
		public static OasisFlyoutController m_flyoutMenuController;

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

		 	BusinessModel.ConnectionString.SetDBPath (GetDatabaseFilePath());
			BusinessModel.Repository.Instance.InitializeDb();

			m_flyoutMenuController = new OasisFlyoutController();
			window.RootViewController = m_flyoutMenuController;
			window.MakeKeyAndVisible ();
			m_loginViewController = new LoginView();
			m_loginViewController.ModalInPopover=false;
			m_loginViewController.ModalTransitionStyle=UIModalTransitionStyle.FlipHorizontal;

			m_flyoutMenuController.PresentViewController (m_loginViewController,false,null);
			return true;
		}

		private string GetDatabaseFilePath ()
		{
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
			string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder instead

			return Path.Combine (libraryPath,"OasisMobile.db3");
		}
	}
}

