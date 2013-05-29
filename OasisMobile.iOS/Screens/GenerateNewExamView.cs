
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using BigTed;
using System.Threading.Tasks;

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

			Console.WriteLine ("Memory warning triggered");
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.Title = "New Exam";
			tblvGenerateExam.Source = new GenerateNewExamTableSource (this);
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
			private UIButton btnExaminationMode;
			private UIButton btnLearningMode;

			public GenerateNewExamTableSource (UIViewController ParentViewController)
			{
				m_currentViewController = ParentViewController;
			}
			
			#region implemented abstract members of UITableViewSource
			
			public override int RowsInSection (UITableView tableview, int section)
			{
				if (section == (int)GenerateExamInterfaceSection.GenerateExamButtons) {
					return 2;
				} else {
					return 1;
				}
			
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				
				UITableViewCell cell;

				switch (indexPath.Section) {
				case (int)GenerateExamInterfaceSection.ExaminationModeExamInfo:
					cell = tableView.DequeueReusableCell ("cell");
					if (cell == null) {
						cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
					}

					cell.TextLabel.Font = UIFont.SystemFontOfSize (13);
					cell.TextLabel.Lines = 0;
					cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
					cell.TextLabel.Text = GetExaminationModeExplanationText (false);
					break;
				case (int)GenerateExamInterfaceSection.LearningModeExamInfo:
					cell = tableView.DequeueReusableCell ("cell");
					if (cell == null) {
						cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
					}

					cell.TextLabel.Font = UIFont.SystemFontOfSize (13);
					cell.TextLabel.Lines = 0;
					cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
					cell.TextLabel.Text = GetExaminationModeExplanationText (true);
					break;
				case (int)GenerateExamInterfaceSection.GenerateExamButtons:
					cell = tableView.DequeueReusableCell ("buttonCell");
					if (cell == null) {
						cell = new UITableViewCell (UITableViewCellStyle.Default, "buttonCell");
					}

					if (indexPath.Row == 0) {
						btnExaminationMode = new UIButton (UIButtonType.RoundedRect);
						btnExaminationMode.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
						btnExaminationMode.Frame = new System.Drawing.RectangleF (0, 0, cell.ContentView.Frame.Width, 36);
						btnExaminationMode.SetTitle ("Start Examination Mode", UIControlState.Normal);
						btnExaminationMode.TouchUpInside += btnExaminationMode_Click;
						cell.ContentView.AddSubview (btnExaminationMode);
					} else {
						btnLearningMode = new UIButton (UIButtonType.RoundedRect);
						btnLearningMode.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
						btnLearningMode.Frame = new System.Drawing.RectangleF (0, 0, cell.ContentView.Frame.Width, 36);
						btnLearningMode.SetTitle ("Start Learning Mode", UIControlState.Normal);
						btnLearningMode.TouchUpInside += btnLearningMode_Click;
						cell.ContentView.AddSubview (btnLearningMode);
					}
					break;
				default:
					cell = tableView.DequeueReusableCell ("cell");
					if (cell == null) {
						cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
					}
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

			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				if (indexPath.Section == (int)GenerateExamInterfaceSection.ExaminationModeExamInfo) {
					SizeF _bounds = new SizeF (tableView.Bounds.Width - 40, float.MaxValue);
					return tableView.StringSize (GetExaminationModeExplanationText (false), UIFont.SystemFontOfSize (13), _bounds, UILineBreakMode.WordWrap).Height + 20; // add 20 px as padding
				} else if (indexPath.Section == (int)GenerateExamInterfaceSection.LearningModeExamInfo) {
					SizeF _bounds = new SizeF (tableView.Bounds.Width - 40, float.MaxValue);
					return tableView.StringSize (GetExaminationModeExplanationText (true), UIFont.SystemFontOfSize (13), _bounds, UILineBreakMode.WordWrap).Height + 20; // add 20 px as padding
				} else {
					return 44;
				}
			}

			public override string TitleForHeader (UITableView tableView, int section)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				string _headerText = "";
				
				switch (section) {
				case (int) GenerateExamInterfaceSection.ExaminationModeExamInfo:
					_headerText = "Examination Mode";
					break;
				case (int) GenerateExamInterfaceSection.LearningModeExamInfo:
					_headerText = "Learning Mode";
					break;
				case (int) GenerateExamInterfaceSection.GenerateExamButtons:
					_headerText = null;
					break;
				}
				return _headerText;
			}

			public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				if (indexPath.Section == (int)GenerateExamInterfaceSection.GenerateExamButtons) {
					cell.BackgroundView = null;
					cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				} else {
					cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				}
				
				
			}

			private string GetExaminationModeExplanationText (bool aIsLearningMode)
			{
				if (aIsLearningMode) {
					return "Self Assessment in the Interactive Mode grades each question as it is answered and submitted. " +
						"You will receive instant feedback, the correct answer, commentary, and references. " +
						"Self Assessments taken in the Learning Mode qualify for CME credits only.";
				} else {
					return "Self Assessment in the Examination Mode qualifies for the MOC requirement of a scored and recorded self assessment. " +
						"Make sure to submit your answer after selection. You must submit your exam before any grading takes place. " +
						"Once you have submitted your answers, you will receive feedback, the correct answer, commentary, and references. " +
						"Self Assessments taken in the Examination Mode qualify for CME and MOC credit.";
				}

			}

			public void btnExaminationMode_Click (object sender, EventArgs e)
			{
				BTProgressHUD.Show ("Downloading Exam", 0);
				bool _isDownloadSuccessful = false;
				Task.Factory.StartNew (() => {
					try {
						WebserviceHelper.GenerateUserExam (false, AppSession.SelectedExam.MainSystemID, AppSession.LoggedInUser.MainSystemID);
						//Use && statement in if so that code gets evaluated only if the previous statement returns true (e.g. if DownloadExamBaseData, we dont need to execute DownloadExamImageFiles)
						if (SyncManager.DownloadExamBaseData (AppSession.SelectedExam) && 
							SyncManager.DownloadExamImageFiles (AppSession.SelectedExam, DownloadExamImageProgressUpdated) && 
							SyncManager.DownloadUserExamCompleteData (AppSession.LoggedInUser, AppSession.SelectedExam)) {
							AppSession.SelectedUserExam = BusinessModel.UserExam.GetFirstUserExamByUserIDAndExamID (
								AppSession.LoggedInUser.UserID,
								AppSession.SelectedExam.ExamID);
							
							AppSession.SelectedUserExam.IsDownloaded = true;
							AppSession.SelectedUserExam.Save ();
							_isDownloadSuccessful = true;
						} else {
							_isDownloadSuccessful = false;
						}
						
					} catch (Exception ex) {
						_isDownloadSuccessful = false;
						Console.WriteLine (ex.ToString ());
					}
			
				}).ContinueWith (task1 => {
					BTProgressHUD.Dismiss ();
					if (_isDownloadSuccessful) {
						if(!AppSession.SelectedUserExam.HasReadDisclosure){
							//Show Disclosure
							ExamDisclosureView _disclosureView = new ExamDisclosureView();
							m_currentViewController.NavigationController.PushViewController (_disclosureView,true);
						}else if(!AppSession.SelectedUserExam.HasReadPrivacyPolicy){
							//Show Privacy Policy
							ExamPrivacyPolicyView _privacyPolicyView = new ExamPrivacyPolicyView();
							m_currentViewController.NavigationController.PushViewController (_privacyPolicyView,true);
						}else{
							//Navigate straight to the exam
							if (UserInterfaceIdiomIsPhone) {
								m_currentViewController.NavigationController.PushViewController (new ExamQuestionList_iPhone (), true);
							} else {
								QuestionSplitView _questionSplitView = new QuestionSplitView ();
								_questionSplitView.PresentAsRootViewWithAnimation ();
							}
						}
					} else {
						UIAlertView _alert = new UIAlertView ("Download Failed", "We could not download your exam right now. Please try again later", null, "Ok", null);
						_alert.Show ();
					}
				}, TaskScheduler.FromCurrentSynchronizationContext ());

			}

			public void btnLearningMode_Click (object sender, EventArgs e)
			{
				BTProgressHUD.Show ("Downloading Exam",0);
				bool _isDownloadSuccessful = false;
				Task.Factory.StartNew (() => {
					try {
						WebserviceHelper.GenerateUserExam (false, AppSession.SelectedExam.MainSystemID, AppSession.LoggedInUser .MainSystemID);
						//Use && statement in if so that code gets evaluated only if the previous statement returns true (e.g. if DownloadExamBaseData, we dont need to execute DownloadExamImageFiles)
						if (SyncManager.DownloadExamBaseData (AppSession.SelectedExam) && 
						    SyncManager.DownloadExamImageFiles (AppSession.SelectedExam,DownloadExamImageProgressUpdated) && 
							SyncManager.DownloadUserExamCompleteData (AppSession.LoggedInUser, AppSession.SelectedExam)) {
							AppSession.SelectedUserExam = BusinessModel.UserExam.GetFirstUserExamByUserIDAndExamID (
								AppSession.LoggedInUser.UserID,
								AppSession.SelectedExam.ExamID);
							
							AppSession.SelectedUserExam.IsDownloaded = true;
							AppSession.SelectedUserExam.Save ();
							_isDownloadSuccessful = true;
						} else {
							_isDownloadSuccessful = false;
						}
						
					} catch (Exception ex) {
						_isDownloadSuccessful = false;
						Console.WriteLine (ex.ToString ());
					}
					
				}).ContinueWith (task1 => {
					BTProgressHUD.Dismiss ();
					if (_isDownloadSuccessful) {
						if(!AppSession.SelectedUserExam.HasReadDisclosure){
							//Show Disclosure
							ExamDisclosureView _disclosureView = new ExamDisclosureView();
							m_currentViewController.NavigationController.PushViewController (_disclosureView,true);
						}else if(!AppSession.SelectedUserExam.HasReadPrivacyPolicy){
							//Show Privacy Policy
							ExamPrivacyPolicyView _privacyPolicyView = new ExamPrivacyPolicyView();
							m_currentViewController.NavigationController.PushViewController (_privacyPolicyView,true);
						}else{
							//Navigate straight to the exam
							if (UserInterfaceIdiomIsPhone) {
								m_currentViewController.NavigationController.PushViewController (new ExamQuestionList_iPhone (), true);
							} else {
								QuestionSplitView _questionSplitView = new QuestionSplitView ();
								_questionSplitView.PresentAsRootViewWithAnimation ();
							}
						}
					} else {
						UIAlertView _alert = new UIAlertView ("Download Failed", "We could not download your exam right now. Please try again later", null, "Ok", null);
						_alert.Show ();
					}
				}, TaskScheduler.FromCurrentSynchronizationContext ());
			}

			public void DownloadExamImageProgressUpdated(float aDownloadProgressPct){
				BTProgressHUD.Show ("Downloading Exam", aDownloadProgressPct);
			}

		}
	}
}

