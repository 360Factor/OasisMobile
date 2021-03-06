using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BigTed;
using System.Net;
using System.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OasisMobile.iOS
{
	public partial class ExamDetailView : UIViewController
	{
		private int m_examID;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public ExamDetailView (int SelectedExamID)
			: base (UserInterfaceIdiomIsPhone ? "ExamDetailView_iPhone" : "ExamDetailView_iPad", null)
		{
			m_examID = SelectedExamID;
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
			// Perform any additional setup after loading the view, typically from a nib.

			this.Title = "Exam Info";
			tblvExamDetail.Source = new ExamDetailTableSource (this, m_examID);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (AppSession.SelectedExam != null) {
				if (tblvExamDetail.Source != null) {
					tblvExamDetail.Source = new ExamDetailTableSource (this, m_examID);
					tblvExamDetail.ReloadData ();
				}
			}


		
			AppSession.SelectedExamUserQuestionList = null;
		}

		public class ExamDetailTableSource : UITableViewSource
		{
			private BusinessModel.ExamAccess m_currentExamAccess;
			private string m_examInfoText;
			private string m_examCreditInfoText;
			private UIButton btnStartContinueExam;
			private UIButton btnDisclosure;
			private UIButton btnPrivacyPolicy;

			public enum ExamDetailSection
			{
				ExamTitle = 0
,
				ExamInfo = 1
,
				CreditInfo = 2
,
				ExamActionButtons = 3
			}

			private UIViewController m_currentViewController;

			public ExamDetailTableSource (UIViewController ParentViewController, int aExamID)
			{
				m_currentViewController = ParentViewController;
				AppSession.SelectedExam = BusinessModel.Exam.GetExamByExamID (aExamID);
				AppSession.SelectedUserExam = BusinessModel.UserExam.GetFirstUserExamByUserIDAndExamID (AppSession.LoggedInUser.UserID, aExamID);
				m_currentExamAccess = BusinessModel.ExamAccess.GetFirstExamAccessByUserIDAndExamID (AppSession.LoggedInUser.UserID,
				                                                                                    AppSession.SelectedExam.ExamID);
				m_examInfoText = GetExamInfoText ();
				m_examCreditInfoText = GetExamCreditInfoText ();
			}
			#region implemented abstract members of UITableViewSource
			
			public override int RowsInSection (UITableView tableview, int section)
			{
				if (section == (int)ExamDetailSection.ExamTitle) {
					return 0;
				} else {
					return 1;
				}
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				
				UITableViewCell cell;

				if (indexPath.Section == (int)ExamDetailSection.ExamActionButtons) {
					cell = tableView.DequeueReusableCell ("buttonRowCell");
					if (cell == null) {
						cell = new CustomTileButtonCell ("buttonRowCell");
					}
					btnStartContinueExam = new UIButton (UIButtonType.RoundedRect);
					//						btnStartContinueExam.Layer.BorderWidth = 1;
					//						btnStartContinueExam.Layer.BorderColor = UIColor.LightGray.CGColor;
					btnStartContinueExam.Layer.ShadowOffset = new SizeF (0, 1);
					btnStartContinueExam.Layer.ShadowRadius = 1;
					btnStartContinueExam.Layer.ShadowOpacity = 0.75f;
					btnStartContinueExam.Layer.ShadowColor = UIColor.DarkGray.CGColor;

					if (AppSession.SelectedUserExam != null && AppSession.SelectedUserExam.IsDownloaded) {
						btnStartContinueExam.SetBackgroundImage (UIImage.FromBundle ("Images/ButtonImages/Icon-Continue.png"), UIControlState.Normal);
					}else{
						btnStartContinueExam.SetBackgroundImage (UIImage.FromBundle ("Images/ButtonImages/Icon-Download.png"), UIControlState.Normal);
					}

					btnStartContinueExam.TouchUpInside += btnStartContinueExam_Clicked;

					btnDisclosure = new UIButton (UIButtonType.RoundedRect);
					//						btnDisclosure.Layer.BorderWidth = 1;
					//						btnDisclosure.Layer.BorderColor = UIColor.LightGray.CGColor;
					btnDisclosure.Layer.ShadowOffset = new SizeF (0, 1);
					btnDisclosure.Layer.ShadowRadius = 1;
					btnDisclosure.Layer.ShadowOpacity = 0.75f;
					btnDisclosure.Layer.ShadowColor = UIColor.DarkGray.CGColor;

					btnDisclosure.SetBackgroundImage (UIImage.FromBundle ("Images/ButtonImages/Icon-Disclosure.png"),UIControlState.Normal);
					btnDisclosure.TouchUpInside += btnDisclosure_Clicked;


					btnPrivacyPolicy = new UIButton (UIButtonType.RoundedRect);
					//						btnPrivacyPolicy.Layer.BorderWidth = 1;
					//						btnPrivacyPolicy.Layer.BorderColor = UIColor.LightGray.CGColor;
					btnPrivacyPolicy.Layer.ShadowOffset = new SizeF (0, 1);
					btnPrivacyPolicy.Layer.ShadowRadius = 1;
					btnPrivacyPolicy.Layer.ShadowOpacity = 0.75f;
					btnPrivacyPolicy.Layer.ShadowColor = UIColor.DarkGray.CGColor;

					btnPrivacyPolicy.SetBackgroundImage (UIImage.FromBundle ("Images/ButtonImages/Icon-PrivacyPolicy.png"),UIControlState.Normal);
					btnPrivacyPolicy.TouchUpInside += btnPrivacyPolicy_Clicked;

					((CustomTileButtonCell)cell).CellButtons  = new UIButton[]{btnStartContinueExam,btnDisclosure,btnPrivacyPolicy};


//					string _buttonTitle;
//					if (AppSession.SelectedUserExam != null && AppSession.SelectedUserExam.IsDownloaded) {
//						_buttonTitle = "Continue Exam";
//					} else {
//						_buttonTitle = "Download Exam";
//					}
//					btnStartContinueExam = new UIButton (UIButtonType.Custom);
					//					btnStartContinueExam.SetImage(new UIImage(UIImage.FromBundle ("Images/ButtonImages/Button-Download.png").CGImage,2,UIImageOrientation.Up),UIControlState.Normal);
//					btnStartContinueExam.TouchUpInside += btnStartContinueExam_Clicked;
//					foreach (UIView _subview in cell.ContentView.Subviews) {
//						_subview.RemoveFromSuperview ();
//					}
//					cell.ContentView.AddSubview (btnStartContinueExam);
				} else {
					cell = tableView.DequeueReusableCell ("cell");
					if (cell == null) {
						cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
					}
					//There is no cell for the Exam Title section as we use the section header for the exam title
					if (indexPath.Section == (int)ExamDetailSection.ExamInfo) {
						UIFont _examInfoFont = UIFont.SystemFontOfSize (13);
						SizeF _bounds = new SizeF (tableView.Bounds.Width - 40, float.MaxValue);
						SizeF _calculatedTextSize = tableView.StringSize (m_examInfoText, _examInfoFont, _bounds, UILineBreakMode.WordWrap);
						cell.TextLabel.Frame = new RectangleF (cell.TextLabel.Frame.X, cell.TextLabel.Frame.Y, _calculatedTextSize.Width, _calculatedTextSize.Height);
						cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
						cell.TextLabel.Lines = 0;
						cell.TextLabel.Font = _examInfoFont;
						cell.TextLabel.Text = m_examInfoText;

					} else if (indexPath.Section == (int)ExamDetailSection.CreditInfo) {
						UIFont _creditInfoFont = UIFont.SystemFontOfSize (13);
						SizeF _bounds = new SizeF (tableView.Bounds.Width - 40, float.MaxValue);
						SizeF _calculatedTextSize = tableView.StringSize (m_examCreditInfoText, _creditInfoFont, _bounds, UILineBreakMode.WordWrap);
						cell.TextLabel.Frame = new RectangleF (cell.TextLabel.Frame.X, cell.TextLabel.Frame.Y, _calculatedTextSize.Width, _calculatedTextSize.Height);
						cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
						cell.TextLabel.Lines = 0;
						cell.TextLabel.Font = _creditInfoFont;
						cell.TextLabel.Text = m_examCreditInfoText;
					}
				}

				//ExamListData _cellExamData = m_userExamTableViewData.ElementAt (indexPath.Section).Value [indexPath.Row];
				
				//cell.TextLabel.Text = _cellExamData.ExamName;
				//cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				
				return cell;
			}
			#endregion
			
			public override int NumberOfSections (UITableView tableView)
			{
				return Enum.GetValues (typeof(ExamDetailSection)).Length;
				// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
			}

			public override string TitleForHeader (UITableView tableView, int section)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				switch (section) {
				case (int)ExamDetailSection.ExamTitle:
					return AppSession.SelectedExam.ExamName;
				case (int)ExamDetailSection.ExamInfo:
					return "Exam Info";
				case (int)ExamDetailSection.CreditInfo:
					return "Credit Info";
				case (int)ExamDetailSection.ExamActionButtons:
					return "";
				default:
					return "";
				}
			}

			public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				if (indexPath.Section == (int)ExamDetailSection.ExamActionButtons) {
					cell.BackgroundView = null;
					cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				} else {
					cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				}
				
				
			}

			public override NSIndexPath WillSelectRow (UITableView tableView, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				if (indexPath.Section == (int)ExamDetailSection.ExamActionButtons) {
					return indexPath;
				} else {
					return null;
				}
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				if (indexPath.Section == (int)ExamDetailSection.ExamActionButtons) {
					tableView.DeselectRow (indexPath, false);
				}
				
			}

			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				if (indexPath.Section == (int)ExamDetailSection.ExamInfo) {
					SizeF _bounds = new SizeF (tableView.Bounds.Width - 40, float.MaxValue);
					return tableView.StringSize (m_examInfoText, UIFont.SystemFontOfSize (13), _bounds, UILineBreakMode.WordWrap).Height + 20; // add 20 px as padding
				} else if (indexPath.Section == (int)ExamDetailSection.ExamActionButtons) {
					return 100;
				} else {
					return 44;
				}
			}

			private string GetExamInfoText ()
			{
				string _examProgressInfo = "";
				string _examModeInfo = "";
				if (AppSession.SelectedUserExam != null) {
					if (AppSession.SelectedUserExam.IsDownloaded) {
						//Progress Info
						Tuple<string,string> _userExamQuestionAnsweredPair = BusinessModel.SQL.Get2Tuples (string.Format (
							"SELECT SUM(HasAnswered), COUNT(*)  FROM tblUserQuestion WHERE fkUserExamID={0}", AppSession.SelectedUserExam.UserExamID)) [0];

						_examProgressInfo = string.Format ("You have answered {0} of {1} questions.",
						                                   _userExamQuestionAnsweredPair.Item1, _userExamQuestionAnsweredPair.Item2);

						if (AppSession.SelectedUserExam.IsSubmitted) {
							_examProgressInfo += " This exam has been submitted.";
						} else {
							_examProgressInfo += " This exam has not been submitted.";
						}

						//Exam mode info
						if (AppSession.SelectedUserExam.IsLearningMode) {
							_examModeInfo = "The exam is taken in learning mode, which qualifies for CME credits.";
						} else {
							_examModeInfo = "The exam is taken in examination mode, which qualifies for CME and MOC credits.";
						}

					} else {
						_examProgressInfo = "The exam has not been downloaded to this device, please download the exam to continue working on the exam.";
					}
				} else {
					//The exam has not been generated yet
					if (m_currentExamAccess != null && m_currentExamAccess.HasAccess) {
						_examProgressInfo = "The exam has not been downloaded to this device, please download the exam to continue working on the exam.";
					} else {
						_examProgressInfo = "You have not purchased this exam, please purchase the exam to work on this exam.";
					}

				}

				if (_examModeInfo != "") {
					return _examProgressInfo + "\n\n" + _examModeInfo;
				} else {
					return _examProgressInfo;
				}

			}

			private string GetExamCreditInfoText ()
			{
				string _examCreditInfoTextToReturn = "";
				if (AppSession.SelectedExam.Credit > 0) {
					if (AppSession.SelectedExam.IsExpired) {
						_examCreditInfoTextToReturn = "This exam has expired and will no longer grant any credit.";
					} else {
						_examCreditInfoTextToReturn = string.Format (
							"This exam is worth {0} CME credits, you can use it for ABOS MOC credits too.",
							AppSession.SelectedExam.Credit);
						if (AppSession.SelectedExam.MinimumPassingScore > 0) {
							_examCreditInfoTextToReturn += string.Format (
								"\n\nYou must answer all questions and score higher than {0}% to request credit.",
								AppSession.SelectedExam.MinimumPassingScore);
						}
					}

				} else {
					_examCreditInfoTextToReturn = "This exam does not grant any credits";
				}

				return _examCreditInfoTextToReturn;
			}

			private void btnStartContinueExam_Clicked (object sender, EventArgs e)
			{
				if (AppSession.SelectedUserExam != null && AppSession.SelectedUserExam.IsDownloaded) {
					if (!AppSession.SelectedUserExam.HasReadDisclosure) {
						ExamDisclosureView _disclosureView = new ExamDisclosureView (true);
						m_currentViewController.NavigationController.PushViewController (_disclosureView, true);
					} else if (!AppSession.SelectedUserExam.HasReadPrivacyPolicy) {
						ExamPrivacyPolicyView _privacyPolicyView = new ExamPrivacyPolicyView (true);
						m_currentViewController.NavigationController.PushViewController (_privacyPolicyView, true);
					} else {
						//Navigate straight to the exam
						if (UserInterfaceIdiomIsPhone) {
							m_currentViewController.NavigationController.PushViewController (new ExamQuestionList_iPhone (), true);
						} else {
							QuestionSplitView _questionSplitView = new QuestionSplitView ();
							_questionSplitView.PresentAsRootViewWithAnimation ();
						}
					}

				
				} else {
					NetworkStatus _internetStatus = Reachability.InternetConnectionStatus ();
					if(_internetStatus == NetworkStatus.NotReachable){
						UIAlertView _alert = new UIAlertView ("No Connection", "Please connect to the internet to download the exam", null, "Ok", null);
						_alert.Show ();
					}else if(_internetStatus ==  NetworkStatus.ReachableViaCarrierDataNetwork){
						UIAlertView _alert = new UIAlertView ("Wifi Connection Required", 
						                                      "Exam material, along with question related images, are too large to download via cellular connection. " +
																"Please find a Wifi connection and re-attempt exam download",
						                                      null, "Ok", null);
						_alert.Show ();
					}else{

						UIAlertView _alert = new UIAlertView ("Wifi Connection Required", 
						                                      "Exam material, along with question related images, takes about 5 to 15 minutes to download. " +
						                                      "Please budget sufficient time for the exam download ",null, "Cancel", "Download");
						_alert.Dismissed += downloadExamAlertView_Dismissed;
						_alert.Show ();
					}
				

				}

			}

			private void downloadExamAlertView_Dismissed(object sender, UIButtonEventArgs e){
				if (e.ButtonIndex == ((UIAlertView) sender).CancelButtonIndex ) {
					//If cancel button is clicked then we return as we dont need to do anything
					return;
				}
				bool _isDownloadSuccessful = false;
				if (AppSession.SelectedUserExam == null) {
					//Direct the user to a specific page to select their exam types
					m_currentViewController.NavigationController.PushViewController (new GenerateNewExamView(), true);
					return;
				}

				BTProgressHUD.Show ("Downloading Exam", 0);
				UIApplication.SharedApplication.IdleTimerDisabled = true;
				Task.Factory.StartNew (() => {
					try{
						if (SyncManager.DownloadExamBaseData (AppSession.SelectedExam) && 
						    SyncManager.DownloadExamImageFiles (AppSession.SelectedExam, DownloadExamImageProgressUpdated) && 
						    SyncManager.DownloadUserExamCompleteData (AppSession.LoggedInUser, AppSession.SelectedExam)) {
							//Update the user exam to session just in case the user exam gets updated in download
							AppSession.SelectedUserExam = BusinessModel.UserExam.GetFirstUserExamByUserIDAndExamID (
								AppSession.LoggedInUser.UserID,
								AppSession.SelectedExam.ExamID);

							AppSession.SelectedUserExam.IsDownloaded = true;
							AppSession.SelectedUserExam.Save ();
							_isDownloadSuccessful = true;
						} else {
							_isDownloadSuccessful = false;
						}
					}
					catch(Exception ex){
						Console.WriteLine (ex.ToString());
					}

				}).ContinueWith (task1 => {
					BTProgressHUD.Dismiss ();
					UIApplication.SharedApplication.IdleTimerDisabled = false;
					if (_isDownloadSuccessful) {
						if(!AppSession.SelectedUserExam.HasReadDisclosure){
							ExamDisclosureView _disclosureView = new ExamDisclosureView(true);
							m_currentViewController.NavigationController.PushViewController (_disclosureView,true);
						}else if(!AppSession.SelectedUserExam.HasReadPrivacyPolicy){
							ExamPrivacyPolicyView _privacyPolicyView = new ExamPrivacyPolicyView(true);
							m_currentViewController.NavigationController.PushViewController (_privacyPolicyView,true);
						}else{
							if(UserInterfaceIdiomIsPhone){
								m_currentViewController.NavigationController.PushViewController (new ExamQuestionList_iPhone (), true);
							}else{
								QuestionSplitView _questionSplitView = new QuestionSplitView();
								_questionSplitView.PresentAsRootViewWithAnimation ();
							}
						}

					} else {
						UIAlertView _alert = new UIAlertView ("Download Failed", "We could not download your exam right now. Please try again later", null, "Ok", null);
						_alert.Show ();
					}
				}, TaskScheduler.FromCurrentSynchronizationContext ());
			}

			private void btnDisclosure_Clicked(object sender, EventArgs e){
				m_currentViewController.NavigationController.PushViewController (new ExamDisclosureView(false),true);
			}

			private void btnPrivacyPolicy_Clicked(object sender, EventArgs e){
				m_currentViewController.NavigationController.PushViewController (new ExamPrivacyPolicyView(false),true);
			}

			public void DownloadExamImageProgressUpdated (float aDownloadProgressPct)
			{
				BTProgressHUD.Show ("Downloading Exam", aDownloadProgressPct);
			}
		}
	}
}

