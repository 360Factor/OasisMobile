// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace OasisMobile.iOS
{
	[Register ("LoginView")]
	partial class LoginView
	{
		[Outlet]
		MonoTouch.UIKit.UITableView tblvLogin { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tblvLogin != null) {
				tblvLogin.Dispose ();
				tblvLogin = null;
			}
		}
	}
}
