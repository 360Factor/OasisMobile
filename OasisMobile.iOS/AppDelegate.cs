using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Threading.Tasks;
using System.Threading;
using BigTed;
using System.Net;

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
		//public static UIViewController m_loginViewController;
		public static OasisFlyoutController m_flyoutMenuController;
		public static NSTimer m_syncTimer;
		public static Thread m_syncBackgroundThread;
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

			AppSettings.SetDefaultSettingsValue ();
		
			BusinessModel.ConnectionString.SetDBPath (GetDatabaseFilePath());
			BusinessModel.Repository.Instance.InitializeDb ();


			if (AppSettings.PersistentLogin && AppSettings.LoggedInLoginName != null && AppSettings.LoggedInLoginName != "") {
				BusinessModel.User _loggedInUser = BusinessModel.User.GetUserByLoginName (AppSettings.LoggedInLoginName);
				AppSession.LoggedInUser = _loggedInUser;
				m_flyoutMenuController = new OasisFlyoutController ();
				window.RootViewController = m_flyoutMenuController;
				window.MakeKeyAndVisible ();
				BTProgressHUD.Show ("Synching Account");
				bool _dataUpdateSuccessful = false;
				Task.Factory.StartNew (() => {
					string responseString ="";
					try{
						WebClient service = new WebClient ();
						service.Headers.Add (HttpRequestHeader.Accept, "application/json");
						string serviceURL = AppConfig.BaseWebserviceURL +
							"UserByLoginName/" + 
								System.Web.HttpUtility.UrlEncode (AppSettings.LoggedInLoginName);
						responseString = service.DownloadString (serviceURL);

						if (responseString != null && responseString != "") {
							var _jsonUser = System.Json.JsonValue.Parse (responseString);

							_loggedInUser = BusinessModel.User.GetUserByMainSystemID (_jsonUser ["UserID"]);
							if (_loggedInUser == null) {
								_loggedInUser = new BusinessModel.User ();
								_loggedInUser.MainSystemID = _jsonUser ["UserID"];
							}
							_loggedInUser.LoginName = _jsonUser ["LoginName"];
							_loggedInUser.UserName = _jsonUser ["UserName"];
							_loggedInUser.Password = _jsonUser ["Password"];
							_loggedInUser.EmailAddress = _jsonUser ["UserEmail"];
							_loggedInUser.LastLoginDate = DateTime.Now;
							_loggedInUser.Save ();
							AppSession.LoggedInUser = _loggedInUser;
							SyncManager.PushAllDoSyncData();
							SyncManager.SyncUserExamDataFromServer (AppSession.LoggedInUser);
							SyncManager.SyncUserExamAccess (AppSession.LoggedInUser);
							_dataUpdateSuccessful = true;
						}else{
							//The user is not found, so we present login view
							AppSettings.LoggedInLoginName=null;
							InvokeOnMainThread (() =>{
								LoginView _loginViewController = new LoginView ();
								_loginViewController.ModalInPopover = false;
								_loginViewController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
								m_flyoutMenuController.PresentViewController (_loginViewController, false, null);
							});
						}

					}
					catch(Exception ex){
						Console.WriteLine (ex.ToString ());
					}

				}).ContinueWith (task1 => {
					BTProgressHUD.Dismiss ();

					if(_dataUpdateSuccessful){
						UIViewController _currentDisplayedController = m_flyoutMenuController.ExamTab.VisibleViewController;
						if(_currentDisplayedController.GetType () == typeof(ExamListView)){
							((ExamListView)_currentDisplayedController).LoadExamList();
						}
					}

					StartSyncThread ();

				}, TaskScheduler.FromCurrentSynchronizationContext ());
			} else {
				m_flyoutMenuController = new OasisFlyoutController ();
				window.RootViewController = m_flyoutMenuController;
				window.MakeKeyAndVisible ();
				LoginView _loginViewController = new LoginView ();
				_loginViewController.ModalInPopover = false;
				_loginViewController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
				m_flyoutMenuController.PresentViewController (_loginViewController, false, null);
				StartSyncThread ();
			}

//			m_flyoutMenuController = new OasisFlyoutController ();
//			window.RootViewController =  m_flyoutMenuController;
//			window.MakeKeyAndVisible ();
//			LoginView _loginViewController = new LoginView ();
//			_loginViewController.ModalInPopover = false;
//			_loginViewController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
//			m_flyoutMenuController.PresentViewController (_loginViewController, false, null);
//			StartSyncThread ();
//
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
			Console.WriteLine ("Starting to create sync thread");
			m_syncBackgroundThread = new Thread (SyncDataInBackground);
			m_syncBackgroundThread.Start ();
		}

		private void SyncDataInBackground ()
		{
			Console.WriteLine ("Sync thread created, running task");
			SyncManager.PushAllDoSyncData ();
			if (AppSession.LoggedInUser != null) {
				SyncManager.SyncUserQuestionAndAnswerFromServer (AppSession.LoggedInUser, false);
			}
			ScheduleNextRun ();
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

