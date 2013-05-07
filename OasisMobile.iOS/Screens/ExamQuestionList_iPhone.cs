
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace OasisMobile.iOS
{
	public partial class ExamQuestionList_iPhone : UIViewController
	{
		public ExamQuestionList_iPhone () : base ("ExamQuestionList_iPhone", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			this.Title = "Questions";
		}
	}
}

