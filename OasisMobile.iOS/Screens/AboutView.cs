
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace OasisMobile.iOS
{
	public partial class AboutView : FlyoutNavigationBaseViewController
	{
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public AboutView ()
			: base (UserInterfaceIdiomIsPhone ? "AboutView_iPhone" : "AboutView_iPad", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			Console.WriteLine ("Memory warning triggered");
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.Title="About";

			tblvAbout.Source = new AboutTableSource (this);
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public class AboutTableSource : UITableViewSource
		{
			private UIViewController m_currentViewController = null;

			private string m_aboutText;
			public AboutTableSource (UIViewController ParentViewController)
			{
				m_currentViewController = ParentViewController;
				m_aboutText ="The AOSSM SelfAssessment and Board Review is the first program of its kind designed " +
					"specifically for orthopedic sports medicine specialists. " +
					"Developed by a faculty of leading practitioners and researchers, " +
					"the program will help you review critical concepts, " +
					"improve clinical decision-making skills and prepare for the new sports-medicine board exam.\n \n" +
					"The AOSSM SelfAssessment iOS App is developed by LightwellSolution. " +
					"For more information, contact info@LightwellSolution.com";
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
				cell.TextLabel.Text = m_aboutText;

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
				return tableView.StringSize (m_aboutText, UIFont.SystemFontOfSize (13), _bounds, UILineBreakMode.WordWrap).Height+20;
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

