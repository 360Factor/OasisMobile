using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace OasisMobile.iOS
{
	public partial class ExamDisclosureView : UIViewController
	{
		private bool m_showAcceptButton;
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public ExamDisclosureView (bool aShowAcceptButton)
			: base (UserInterfaceIdiomIsPhone ? "ExamDisclosureView_iPhone" : "ExamDisclosureView_iPad", null)
		{
			m_showAcceptButton = aShowAcceptButton;
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
			this.Title = "Disclosure";


			tblvExamDisclosure.Source = new ExamDisclosureTableSource (this, m_showAcceptButton);
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public class ExamDisclosureTableSource : UITableViewSource
		{
			private UIViewController m_currentViewController = null;
			private UIButton btnAcceptAndContinue;
			private bool m_showAcceptButton;

			public ExamDisclosureTableSource (UIViewController ParentViewController, bool aShowAcceptButton)
			{
				m_currentViewController = ParentViewController;
				m_showAcceptButton = aShowAcceptButton;

			}
			#region implemented abstract members of UITableViewSource

			public override int RowsInSection (UITableView tableview, int section)
			{
				if (section == 0) {
					return 1; //Section 0 contains the Disclosure
				} else {
					// Section 1 the Accept Button
					if (m_showAcceptButton) {
						return 1; 
					} else {
						return 0;
					}
				}
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell;

				if (indexPath.Section == 0) {
					cell = tableView.DequeueReusableCell ("cell");
					if (cell == null) {
						cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
					}

					cell.TextLabel.Font = UIFont.SystemFontOfSize (13);
					cell.TextLabel.Lines = 0;
					cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
					cell.TextLabel.Text = AppSession.SelectedExam.Disclosure;
				} else {
					cell = tableView.DequeueReusableCell ("buttonCell");
					if (cell == null) {
						cell = new UITableViewCell (UITableViewCellStyle.Default, "buttonCell");
					}
					if (btnAcceptAndContinue == null) {
						btnAcceptAndContinue = new UIButton (UIButtonType.RoundedRect);
						btnAcceptAndContinue.Frame = cell.ContentView.Bounds;
						btnAcceptAndContinue.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
						btnAcceptAndContinue.SetTitle ("I Agree",UIControlState.Normal);
						btnAcceptAndContinue.TouchUpInside += btnAcceptAndContinue_Click;
						cell.ContentView.AddSubview (btnAcceptAndContinue);
					}
		
				}

				return cell;
			}
			#endregion

			public override int NumberOfSections (UITableView tableView)
			{
				return 2; //One section is the disclosure text, another one is the button
				// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 

				tableView.DeselectRow (indexPath,false);
			}

			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				if (indexPath.Section == 0) {
					SizeF _bounds = new SizeF (tableView.Bounds.Width - 40, float.MaxValue);
					return tableView.StringSize (AppSession.SelectedExam.Disclosure, UIFont.SystemFontOfSize (13),
					                             _bounds,UILineBreakMode.WordWrap).Height + 20;
				} else {
					return 44;
				}
			
			}

			public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				if (indexPath.Section == 0) {
					cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				}
			}

			private void btnAcceptAndContinue_Click (object sender, EventArgs e)
			{
				AppSession.SelectedUserExam.HasReadDisclosure = true;
				AppSession.SelectedUserExam.DoSync = true;
				AppSession.SelectedUserExam.Save ();
				if (AppSession.SelectedUserExam.HasReadPrivacyPolicy) {
					if (UserInterfaceIdiomIsPhone) {
						m_currentViewController.NavigationController.PushViewController (new ExamQuestionList_iPhone (), true);
					} else {
						QuestionSplitView _questionSplitView = new QuestionSplitView ();
					
						_questionSplitView.PresentAsRootViewWithAnimation ();
					}
				} else {
					var _privacyPolicyView = new ExamPrivacyPolicyView (true);
					m_currentViewController.NavigationController.PushViewController (_privacyPolicyView, true);
				}
			}
		}
	}
}

