// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace OasisMobile.iOS
{
	[Register ("ExamQuestionList_iPhone")]
	partial class ExamQuestionList_iPhone
	{
		[Outlet]
		MonoTouch.UIKit.UITableView tblvExamQuestionList { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tblvExamQuestionList != null) {
				tblvExamQuestionList.Dispose ();
				tblvExamQuestionList = null;
			}
		}
	}
}
