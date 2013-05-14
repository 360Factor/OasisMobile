using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Threading.Tasks;

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
		public static NSTimer m_syncTimer;
		public static Task m_syncBackgroundTask;
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
			BusinessModel.Repository.Instance.InitializeDb ();

			m_flyoutMenuController = new OasisFlyoutController ();
			window.RootViewController = m_flyoutMenuController;
			window.MakeKeyAndVisible ();
			m_loginViewController = new LoginView ();
			m_loginViewController.ModalInPopover = false;
			m_loginViewController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;

			m_flyoutMenuController.PresentViewController (m_loginViewController, false, null);

			StartSyncThread ();
			return true;
		}

		private string GetDatabaseFilePath ()
		{
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
			string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder instead

			return Path.Combine (libraryPath, "OasisMobile.db3");
		}

		private void StartSyncThread ()
		{
			m_syncBackgroundTask = Task.Factory.StartNew (()=>{
					SyncManager.PushAllDoSyncData ();
					if(AppSession.LoggedInUser != null){
						SyncManager.SyncUserQuestionAndAnswerFromServer (AppSession.LoggedInUser, false);
					}

					ScheduleNextRun ();
				});
		}

		private void ScheduleNextRun ()
		{
			using (var pool = new NSAutoreleasePool()) {
				m_syncTimer = NSTimer.CreateScheduledTimer (300, delegate { 
					SyncManager.PushAllDoSyncData ();
					ScheduleNextRun ();
				});
				NSRunLoop.Current.Run ();
			}
		
		}

		public override void DidEnterBackground (UIApplication application)
		{
			// NOTE: Don't call the base implementation on a Model class
			// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
			if (m_syncTimer != null && m_syncTimer.IsValid) {
				//Invalidate
				m_syncTimer.Invalidate ();
				m_syncTimer = null; //release the reference to the timer
			}
			//try do do a data push once in the background to make sure data is all synched
			int _backgroundTaskID = UIApplication.SharedApplication.BeginBackgroundTask (() => {});
			Task.Factory.StartNew (()=>{
				SyncManager.PushAllDoSyncData ();
				UIApplication.SharedApplication.EndBackgroundTask (_backgroundTaskID);
			});
		}

		public override void WillEnterForeground (UIApplication application)
		{
			// NOTE: Don't call the base implementation on a Model class
			// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
			StartSyncThread ();
		}
	}
}

