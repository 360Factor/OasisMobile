// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace OasisMobile.iOS
{
	[Register ("Question_iPhone")]
	partial class Question_iPhone
	{
		[Outlet]
		MonoTouch.UIKit.UIScrollView svQuestionPager { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (svQuestionPager != null) {
				svQuestionPager.Dispose ();
				svQuestionPager = null;
			}
		}
	}
}
