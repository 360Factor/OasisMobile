
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace OasisMobile.iOS
{
	public partial class GenerateNewExamView : UIViewController
	{
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public GenerateNewExamView ()
			: base (UserInterfaceIdiomIsPhone ? "GenerateNewExamView_iPhone" : "GenerateNewExamView_iPad", null)
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
			tblvGenerateExam.Source = new GenerateNewExamTableSource(this);
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public class GenerateNewExamTableSource : UITableViewSource
		{
			public enum GenerateExamInterfaceSection
			{
				ExaminationModeExamInfo = 0,
				LearningModeExamInfo = 1,
				GenerateExamButtons = 2
			}

			private UIViewController m_currentViewController = null;
			
			public GenerateNewExamTableSource (UIViewController ParentViewController)
			{
				m_currentViewController = ParentViewController;
			}
			
			#region implemented abstract members of UITableViewSource
			
			public override int RowsInSection (UITableView tableview, int section)
			{
				return 1;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				
				UITableViewCell cell;
				
				cell = tableView.DequeueReusableCell ("cell");
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
				}

				switch(indexPath.Section){
				case (int) GenerateExamInterfaceSection.ExaminationModeExamInfo:
					break;
				case (int) GenerateExamInterfaceSection.LearningModeExamInfo:
					break;
				case (int) GenerateExamInterfaceSection.GenerateExamButtons:
					break;
				}
				
				return cell;
			}
			
			#endregion
			
			public override int NumberOfSections (UITableView tableView)
			{
				return Enum.GetValues (typeof(GenerateExamInterfaceSection)).Length;
				// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				
				tableView.DeselectRow (indexPath, false);
			}
			
			
		}
	}
}

