using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace OasisMobile.iOS
{
	public partial class ExamDisclosureView : UIViewController
	{
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public ExamDisclosureView ()
			: base (UserInterfaceIdiomIsPhone ? "ExamDisclosureView_iPhone" : "ExamDisclosureView_iPad", null)
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
			this.Title = "Disclosure";


			tblvExamDisclosure.Source = new ExamDisclosureTableSource (this);
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public class ExamDisclosureTableSource : UITableViewSource
		{
			private UIViewController m_currentViewController = null;
			private UIButton btnAcceptAndContinue;

			public ExamDisclosureTableSource (UIViewController ParentViewController)
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

				if (indexPath.Section == 0) {
					cell = tableView.DequeueReusableCell ("cell");
					if (cell == null) {
						cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
					}

					cell.TextLabel.Font = UIFont.SystemFontOfSize (14);
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
						AppDelegate.window.RootViewController = _questionSplitView;

						UIView.Transition (AppDelegate.window,
						                   0.5,
						                   UIViewAnimationOptions.TransitionFlipFromRight,
						                   () => {
							AppDelegate.window.RootViewController = _questionSplitView;
							_questionSplitView.WillAnimateRotation (m_currentViewController.InterfaceOrientation, 0);
						}, null);
					}
				} else {
					var _privacyPolicyView = new ExamPrivacyPolicyView ();
					m_currentViewController.NavigationController.PushViewController (_privacyPolicyView, true);
				}
			}
		}
	}
}

