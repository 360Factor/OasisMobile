using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace OasisMobile.iOS
{
	public partial class SupportView : FlyoutNavigationBaseViewController
	{
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public SupportView ()
			: base (UserInterfaceIdiomIsPhone ? "SupportView_iPhone" : "SupportView_iPad", null)
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
			this.Title = "Support";
			tblvSupport.Source = new SupportTableSource (this);
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public class SupportTableSource : UITableViewSource
		{
			private UIViewController m_currentViewController = null;

			private string m_supportText;
			public SupportTableSource (UIViewController ParentViewController)
			{
				m_currentViewController = ParentViewController;
				m_supportText ="For questions about AOSSM SelfAssessment program, please contact AOSSM at 847/292-4900, " +
						"or visit us at http://www.sportsmed.org.\n \n" +
						"To obtain CME / MOC credit:\n \n" +
						"1.Complete answers to all 125 questions\n" +
						"2.Access AOSSM's SelfAssessment Exam website at http://selfassessment.sportsmed.org and " +
						"complete the evaluation for the exam, " +
						"including the number of hours you spent answering questions and reviewing answers.\n" +
						"3.Press \"Request CME\" at the end screen.";
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

				cell.TextLabel.Font = UIFont.SystemFontOfSize (13);
				cell.TextLabel.Lines = 0;
				cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
				cell.TextLabel.Text = m_supportText;
			
				return cell;
			}

			#endregion

			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
				// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				tableView.DeselectRow (indexPath, false);
			}

			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				SizeF _bounds = new SizeF (tableView.Bounds.Width - 40, float.MaxValue);
				return tableView.StringSize (m_supportText, UIFont.SystemFontOfSize (13), _bounds, UILineBreakMode.WordWrap).Height+20;
			}

			public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			}


		}
	}
}

