using System;
using System.Collections.Generic;
using MonoTouch.Foundation;


namespace OasisMobile.iOS
{
	public static class AppSettings
	{
		public static bool PersistentLogin{
			get{
				return NSUserDefaults.StandardUserDefaults.BoolForKey ("PersistentLogin");
			}
			set{
				NSUserDefaults.StandardUserDefaults.SetBool (value,"PersistentLogin");
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			}
		}

		public static bool AutoAdvanceQuestion{
			get{
				return NSUserDefaults.StandardUserDefaults.BoolForKey ("AutoAdvanceQuestion");
			}
			set{
				NSUserDefaults.StandardUserDefaults.SetBool (value,"AutoAdvanceQuestion");
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			}
		}

		public static bool AutoSubmitResponse{
			get{
				return NSUserDefaults.StandardUserDefaults.BoolForKey ("AutoSubmitResponse");
			}
			set{
				NSUserDefaults.StandardUserDefaults.SetBool (value,"AutoSubmitResponse");
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			}
		}

		public static string LoggedInLoginName{
			get{
				return NSUserDefaults.StandardUserDefaults.StringForKey ("LoggedInLoginName");
			} 
			set{
				NSUserDefaults.StandardUserDefaults.SetString (value,"LoggedInLoginName");
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			}
		}

		public static void SetDefaultSettingsValue(){
			var _settingValues = new Object [] { true, true, false };
			var _settingKeys = new Object [] { "PersistentLogin", "AutoAdvanceQuestion", "AutoSubmitResponse" };
			var _appDefaults = NSDictionary.FromObjectsAndKeys (_settingValues, _settingKeys);
			NSUserDefaults _userSettings = NSUserDefaults.StandardUserDefaults;
			_userSettings.RegisterDefaults (_appDefaults);
		}
	}
}

