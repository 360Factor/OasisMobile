// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace OasisMobile.iOS
{
	[Register ("ExamDisclosureView")]
	partial class ExamDisclosureView
	{
		[Outlet]
		MonoTouch.UIKit.UITableView tblvExamDisclosure { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tblvExamDisclosure != null) {
				tblvExamDisclosure.Dispose ();
				tblvExamDisclosure = null;
			}
		}
	}
}
