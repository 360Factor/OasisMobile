// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace OasisMobile.iOS
{
	[Register ("AboutView")]
	partial class AboutView
	{
		[Outlet]
		MonoTouch.UIKit.UITableView tblvAbout { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tblvAbout != null) {
				tblvAbout.Dispose ();
				tblvAbout = null;
			}
		}
	}
}
