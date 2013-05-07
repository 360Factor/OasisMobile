
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BigTed;
using System.Net;
using System.Json;
using System.Collections.Generic;
using System.Linq;

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

				} else {
					DownloadBaseExam (AppSession.SelectedExam);
				}
				m_currentViewController.NavigationController.PushViewController (new ExamQuestionList_iPhone (), true);
			}

			private bool DownloadBaseExam (BusinessModel.Exam aExam)
			{
				string _serviceTargetURL = "";
				string _responseString = "";
				WebClient _service = new WebClient ();

				//Since categories are not expected to change in the lifetime of the application, 
				//we will just pull the the category once and resync categories when a category does not exist in the downloaded exam question
				Dictionary<int,int> _mainSystemIDToCategoryIDMap = BusinessModel.SQL.ExecuteIntIntDictionary ("SELECT MainSystemID, pkCategoryID FROM Category");
				if (_mainSystemIDToCategoryIDMap.Count == 0) {
					//If there are no categories yet, we download the categories from the webservice
					_mainSystemIDToCategoryIDMap = DownloadCategories ();
					if (_mainSystemIDToCategoryIDMap == null) {
						//If the connection fails while downloading categories, we will return false to indicate that the download has failed
						return false;
					}

				}


				//Download exam base question data (question/image record/ answer option)
				_serviceTargetURL = AppConfig.BaseWebserviceURL + string.Format (
					"GetCompleteExamQuestionData/{0}", aExam.MainSystemID);

				try {
					_service.Headers.Add (HttpRequestHeader.Accept, "application/json");
					_responseString = _service.DownloadString (_serviceTargetURL);
				} catch (Exception ex) {
					return false;
				}

				if (_responseString != null && _responseString != "") {
					JsonValue _jsonObj = JsonValue.Parse (_responseString);

					// Sync Exam Base Question
					//---------------------------
					List<BusinessModel.Question> _localQuestionList = BusinessModel.Question.GetQuestionsByExamID (aExam.ExamID);
					
					if(_localQuestionList == null){
						_localQuestionList = new List<BusinessModel.Question>();
					}

					JsonArray _remoteQuestionList = (JsonArray) _jsonObj ["QuestionList"];
					foreach (JsonValue _remoteQuestion in _remoteQuestionList) {
						BusinessModel.Question _matchinglocalQuestion = 
							(from x in _localQuestionList 
							 where x.MainSystemID == _remoteQuestion["QuestionID"] select x).FirstOrDefault ();
						if(_matchinglocalQuestion == null){
							_matchinglocalQuestion = new BusinessModel.Question();
							_matchinglocalQuestion.MainSystemID = _remoteQuestion["QuestionID"];
							_localQuestionList.Add (_matchinglocalQuestion);
						}
						_matchinglocalQuestion.Stem = _remoteQuestion["Stem"];
						_matchinglocalQuestion.LeadIn = _remoteQuestion["LeadIn"];
						_matchinglocalQuestion.Commentary = _remoteQuestion["Commentary"];
						_matchinglocalQuestion.Reference = _remoteQuestion["Reference"];
						_matchinglocalQuestion.ExamID = aExam.ExamID;
						if(_mainSystemIDToCategoryIDMap.ContainsKey (_remoteQuestion["CategoryID"])){
							_matchinglocalQuestion.CategoryID = _mainSystemIDToCategoryIDMap[_remoteQuestion["CategoryID"]];
						}
						else{
							//If there are categories used by question that does not exist yet, we resync the category
							_mainSystemIDToCategoryIDMap = DownloadCategories();
							if(_mainSystemIDToCategoryIDMap == null){
								//If for some reason, the download of categories fail, we cannot continue and so return false for failing to download exam
								return false;
							}
							_matchinglocalQuestion.CategoryID = _mainSystemIDToCategoryIDMap[_remoteQuestion["CategoryID"]];
						}
						_matchinglocalQuestion.PopulationCorrectPct = _remoteQuestion["OverallCorrectResponsePct"];
					}
					BusinessModel.Question.SaveAll (_localQuestionList);


					//After all question is updated, create a mapping between mainsystemid and localquestion id
					Dictionary<int,int> _mainSystemIDToQuestionIDMap = 
						(from x in _localQuestionList 
						 select new{MainSystemID = x.MainSystemID, 
									QuestionID = x.QuestionID}).ToDictionary(y=>y.MainSystemID, y=>y.QuestionID);


					// SYNC EXAM BASE ANSWER OPTION
					//-----------------------------------------
					List<BusinessModel.AnswerOption> _localAnswerOptionList = 
						BusinessModel.AnswerOption.GetAnswerOptionsBySQL (string.Format (
							"SELECT * FROM AnswerOption INNER JOIN Question " +
							"ON AnswerOption.fkQuestionID = Question.pkQuestionID " +
							"WHERE Question.fkExamID={0}",aExam.ExamID));
					
					if(_localAnswerOptionList == null){
						_localAnswerOptionList = new List<BusinessModel.AnswerOption>();
					}

					JsonArray _remoteAnswerOptionList = (JsonArray)_jsonObj["AnswerOptionList"];
					foreach(JsonValue _remoteAnswerOption in _remoteQuestionList){
						BusinessModel.AnswerOption _matchingLocalAnswerOption = 
							(from x in _localAnswerOptionList where x.MainSystemID == _remoteAnswerOption["AnswerOptionID"] 
							 select x).FirstOrDefault ();
						if(_matchingLocalAnswerOption == null){
							_matchingLocalAnswerOption = new BusinessModel.AnswerOption();
							_matchingLocalAnswerOption.MainSystemID = _remoteAnswerOption["AnswerOptionID"];
							_localAnswerOptionList.Add (_matchingLocalAnswerOption);
						}
						_matchingLocalAnswerOption.QuestionID = _mainSystemIDToQuestionIDMap[_remoteAnswerOption["QuestionID"]];
						_matchingLocalAnswerOption.AnswerText = _remoteAnswerOption["Text"];
						_matchingLocalAnswerOption.IsCorrect = _remoteAnswerOption["IsCorrect"];
					}
					BusinessModel.AnswerOption.SaveAll (_localAnswerOptionList);

					// SYNC EXAM BASE IMAGES
					//-----------------------------------------
					List<BusinessModel.Image> _localImageList = 
						BusinessModel.Image.GetImagesBySQL (string.Format (
							"SELECT Image.* FROM Image INNER JOIN Question " +
							"ON Image.fkQuestionID = Question.pkQuestionID " +
							"WHERE Question.fkExamID={0}",aExam.ExamID));
					
					if(_localImageList == null){
						_localImageList = new List<BusinessModel.Image>();
					}
					
					JsonArray _remoteImageList = (JsonArray) _jsonObj["QuestionImageMap"];
					foreach(JsonValue _remoteImage in _remoteImageList){
						BusinessModel.Image _matchingLocalImage = 
							(from x in _localImageList where x.MainSystemID == _remoteImage["ImageID"] 
							 select x).FirstOrDefault ();
						if(_matchingLocalImage == null){
							_matchingLocalImage = new BusinessModel.Image();
							_matchingLocalImage.MainSystemID = _remoteImage["ImageID"];
							_localImageList.Add (_matchingLocalImage);
						}
						_matchingLocalImage.QuestionID = _mainSystemIDToQuestionIDMap[_remoteImage["QuestionID"]];
						_matchingLocalImage.Title = _remoteImage["FigureTitle"];
						_matchingLocalImage.ShowInQuestion = _remoteImage["ShowInQuestion"];
						_matchingLocalImage.ShowInCommentary = _remoteImage["ShowInCommentary"];
						_matchingLocalImage.DownloadURL = "";

					}
					BusinessModel.Image.SaveAll (_localImageList);

				} else {
					return false;
				}

		


				return true;
			}

			/// <summary>
			/// Downloads the categories. Returns a dictionary of MainSystemID to local CategoryID or null if the connection fails
			/// </summary>
			/// <returns>A mapping of MainSystemID to the local CategoryID.</returns>
			private Dictionary<int, int> DownloadCategories ()
			{
				List<BusinessModel.Category> _localCategoryList = BusinessModel.Category.GetAllCategorys ();
				Dictionary<int, BusinessModel.Category> _mainSystemIDToCategoryObjMap = new Dictionary<int, OasisMobile.BusinessModel.Category> ();
				foreach (BusinessModel.Category _localCategory in _localCategoryList) {
					_mainSystemIDToCategoryObjMap.Add (_localCategory.MainSystemID, _localCategory);
				}

				//We dont have any categories yet, we will download the categories now.
				string _serviceTargetURL = AppConfig.BaseWebserviceURL + "GetCategories";
				string _responseString = "";
				try {
					WebClient _service = new WebClient ();
					_service.Headers.Add (HttpRequestHeader.Accept, "application/json");
					_responseString = _service.DownloadString (_serviceTargetURL);
				} catch (Exception ex) {
					//Return right away when an exception occurs because that means the get has failed and no data should be returned  
					return null;
				}
				if (_responseString != null && _responseString != "") {
					JsonArray _remoteCategoryList = (JsonArray) JsonValue.Parse (_responseString);

					// FIRST SAVE LOOP
					//----------------------
					foreach (JsonValue _remoteCategory in _remoteCategoryList) {
						BusinessModel.Category _matchingLocalCategory;
						if (_mainSystemIDToCategoryObjMap.ContainsKey (_remoteCategory ["CategoryID"])) {
							_matchingLocalCategory = _mainSystemIDToCategoryObjMap [_remoteCategory ["CategoryID"]];
							
						} else {
							_matchingLocalCategory = new BusinessModel.Category ();
							_matchingLocalCategory.MainSystemID = _remoteCategory ["CategoryID"];
							//Set the parent category as null for the first run as we will do another run with all categories saved so we have the local id to map to
							_matchingLocalCategory.ParentCategoryID = null; 
							
							_localCategoryList.Add (_matchingLocalCategory);
							_mainSystemIDToCategoryObjMap.Add (_matchingLocalCategory.MainSystemID, _matchingLocalCategory);
							
						}
						_matchingLocalCategory.CategoryName = _remoteCategory ["CategoryName"];
						_matchingLocalCategory.ExpandedCategoryName = _remoteCategory ["ExpandedCategoryName"];
						_matchingLocalCategory.DisplayOrder = _remoteCategory ["DisplayOrder"];
					}
					
					BusinessModel.Category.SaveAll (_localCategoryList);

					// SECOND SAVE LOOP
					//-----------------------

					//After all is saved, loop through the list to create a mapping of MainSystemID to CategoryID
					Dictionary<int,int> _mainSystemIDToLocalIDCategoryMap = new Dictionary<int, int> ();

					//Then we need to loop the list a second time to populate the parent category id, this is because in the first loop, 
					//the new categories are not yet assigned an ID and so cannot be parent categories yet until they are saved
					//Also use this loop to generate the mainsystem to local id category map
					foreach (JsonValue _remoteCategory in _remoteCategoryList) {
						BusinessModel.Category _matchingLocalCategory = _mainSystemIDToCategoryObjMap [_remoteCategory ["CategoryID"]];
						_matchingLocalCategory.ParentCategoryID = _mainSystemIDToCategoryObjMap [_remoteCategory ["ParentID"]].CategoryID;
						_mainSystemIDToLocalIDCategoryMap.Add (_matchingLocalCategory.MainSystemID, _matchingLocalCategory.CategoryID);
					}
					BusinessModel.Category.SaveAll (_localCategoryList);

					return _mainSystemIDToLocalIDCategoryMap;
				}
				else{
					return null;
				}
			}
		}

	}
}

