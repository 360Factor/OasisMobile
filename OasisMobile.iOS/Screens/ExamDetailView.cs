
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
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			this.Title = "Exam Info";

			tblvExamDetail.Source = new ExamDetailTableSource (this, m_examID);
//			using (MonoTouch.CoreGraphics.CGContext g = UIGraphics.GetCurrentContext ()) {
//				g.SetLineWidth (5);
//				g.SetStrokeColor (10,10,10,1);
//				g.MoveTo (0,50);
//				g.AddLineToPoint (this.View.Frame.Width,50);
//				g.StrokePath ();
//			}
		}

	

		public class ExamDetailTableSource : UITableViewSource
		{
			private BusinessModel.ExamAccess m_currentExamAccess;
			private string m_examInfoText;
			private string m_examCreditInfoText;
			private UIButton btnStartContinueExam;
			public enum ExamDetailSection
			{
				ExamTitle = 0,
				ExamInfo = 1,
				CreditInfo = 2,
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
						cell = new UITableViewCell (UITableViewCellStyle.Default, "buttonRowCell");
					}

					string _buttonTitle;
					if (AppSession.SelectedUserExam != null && AppSession.SelectedUserExam.IsDownloaded) {
						_buttonTitle = "Continue Exam";
					} else {
						_buttonTitle = "Download Exam";
					}
					btnStartContinueExam = new UIButton (UIButtonType.RoundedRect);
					btnStartContinueExam.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
					btnStartContinueExam.Frame = new System.Drawing.RectangleF (0, 0, cell.Frame.Width, 36);
					btnStartContinueExam.SetTitle (_buttonTitle, UIControlState.Normal);
					btnStartContinueExam.TouchUpInside += btnStartContinueExam_Clicked;
					cell.ContentView.AddSubview (btnStartContinueExam);
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
							"SELECT SUM(HasAnswered), COUNT(*)  FROM UserQuestion WHERE fkUserExamID={0}", AppSession.SelectedUserExam.UserExamID)) [0];

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
					//Navigate straight to the exam
					m_currentViewController.NavigationController.PushViewController (new ExamQuestionList_iPhone (), true);
				} else {
					bool _isDownloadSuccessful = false;
					if (AppSession.SelectedUserExam == null) {
						//Direct the user to a specific page to select their exam types
						m_currentViewController.NavigationController.PushViewController (new GenerateNewExamView(),true);
						return;
					}

					BTProgressHUD.Show ("Downloading Exam", 0);
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
						if (_isDownloadSuccessful) {
							m_currentViewController.NavigationController.PushViewController (new ExamQuestionList_iPhone (), true);
						} else {
							UIAlertView _alert = new UIAlertView ("Download Failed", "We could not download your exam right now. Please try again later", null, "Ok", null);
							_alert.Show ();
						}
					}, TaskScheduler.FromCurrentSynchronizationContext ());
				}

			}

			public void DownloadExamImageProgressUpdated(float aDownloadProgressPct){
				BTProgressHUD.Show ("Downloading Exam", aDownloadProgressPct);
			}

		}

	}
}

