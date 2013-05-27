using System;
using System.Collections.Generic;
using System.Net;
using System.Json;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
namespace OasisMobile.iOS
{
	public static class SyncManager
	{
		public static List<BusinessModel.Exam> SyncExamDataFromServer ()
		{
			//Get local exams
			List<BusinessModel.Exam> _localExamList = BusinessModel.Exam.GetAllExams ();
			if (_localExamList == null) {
				_localExamList = new List<BusinessModel.Exam> ();
			}

			//Get published remote exam
			string _serviceTargetURL = AppConfig.BaseWebserviceURL + "ExamsIsPublished";
			WebClient _service = new WebClient ();
			_service.Headers.Add (HttpRequestHeader.Accept, "application/json");
			string _responseString = _service.DownloadString (_serviceTargetURL);
			if (_responseString != null && _responseString != "") {
				var _jsonResponseArr = (JsonArray)JsonValue.Parse (_responseString);

				foreach (JsonValue _remoteExam in _jsonResponseArr) {
					BusinessModel.Exam _examToSave = 
						(from x in _localExamList where x.MainSystemID == _remoteExam ["ExamID"] select x).FirstOrDefault ();
					if (_examToSave == null) {
						_examToSave = new BusinessModel.Exam ();
						_examToSave.MainSystemID = _remoteExam ["ExamID"];
						_localExamList.Add (_examToSave);
					}
					_examToSave.ExamName = _remoteExam ["Name"];
					_examToSave.ExamTypeID = _remoteExam ["ExamTypeID"];
					_examToSave.Description = _remoteExam ["ExamDescription"];
					_examToSave.Credit = _remoteExam ["Credit"];
					_examToSave.IsExpired = false;
					_examToSave.MinimumPassingScore = _remoteExam ["MinimumPassingScore"];
					_examToSave.Price = _remoteExam ["Price"];
					_examToSave.PrivacyPolicy = _remoteExam ["PrivacyPolicy"];
					_examToSave.Disclosure = _remoteExam ["Disclosure"];


					//Trim the exam's disclosure and privacy policy
					//------------------------------------------------

					//Replace 2 or more line break to 2 line breaks
					_examToSave.PrivacyPolicy = Regex.Replace (_examToSave.PrivacyPolicy, "(<br\\s*\\/*>\\s*){2,}", "<br /><br />");
					_examToSave.Disclosure = Regex.Replace (_examToSave.Disclosure, "(<br\\s*\\/*>\\s*){2,}", "<br /><br />");

					_examToSave.PrivacyPolicy = Regex.Replace (_examToSave.PrivacyPolicy, "<br\\s*\\/*>", "\n");
					_examToSave.Disclosure = Regex.Replace (_examToSave.Disclosure, "<br\\s*\\/*>", "\n");

					_examToSave.PrivacyPolicy = Regex.Replace (_examToSave.PrivacyPolicy, "<div[^>]*>(.+)</\\s*div>", "$1\n");
					_examToSave.Disclosure = Regex.Replace (_examToSave.Disclosure, "<div[^>]*>(.+)</\\s*div>", "$1\n");

					_examToSave.PrivacyPolicy = Regex.Replace (_examToSave.PrivacyPolicy, "<[^>]+>(.+)<\\/[^>]+>","$1");
					_examToSave.Disclosure = Regex.Replace (_examToSave.Disclosure, "<[^>]+>(.+)<\\/[^>]+>","$1");

				}
			}

			//Get expired remote exam
			_serviceTargetURL = AppConfig.BaseWebserviceURL + "ExamsIsExpired";
			_service.Headers.Add (HttpRequestHeader.Accept, "application/json");
			_responseString = _service.DownloadString (_serviceTargetURL);
			if (_responseString != null && _responseString != "") {
				var _jsonResponseArr = (JsonArray)JsonValue.Parse (_responseString);
				
				foreach (JsonValue _remoteExam in _jsonResponseArr) {
					BusinessModel.Exam _examToSave = 
						(from x in _localExamList where x.MainSystemID == _remoteExam ["ExamID"] select x).FirstOrDefault ();
					if (_examToSave == null) {
						_examToSave = new BusinessModel.Exam ();
						_examToSave.MainSystemID = _remoteExam ["ExamID"];
						_localExamList.Add (_examToSave);
					}
					_examToSave.ExamName = _remoteExam ["Name"];
					_examToSave.Description = _remoteExam ["ExamDescription"];
					_examToSave.Credit = _remoteExam ["Credit"];
					_examToSave.IsExpired = true;
					_examToSave.MinimumPassingScore = _remoteExam ["MinimumPassingScore"];
					_examToSave.Price = _remoteExam ["Price"];
					_examToSave.PrivacyPolicy = _remoteExam ["PrivacyPolicy"];
					_examToSave.Disclosure = _remoteExam ["Disclosure"];
				}
			}

			//Save updated list
			BusinessModel.Exam.SaveAll (_localExamList);

			return _localExamList;
		}

		public static List<BusinessModel.UserExam> SyncUserExamDataFromServer (BusinessModel.User aUser)
		{
			//Create the Exam main system id to local id mapping
			Dictionary<int,int> _mainSystemExamIDToLocalExamIDMap = 
				BusinessModel.SQL.ExecuteIntIntDictionary ("SELECT MainSystemID, pkExamID FROM tblExam");

			//Get Local User Exams
			List<BusinessModel.UserExam> _localUserExamList = BusinessModel.UserExam.GetUserExamsByUserID (aUser.UserID);
			if (_localUserExamList == null) {
				_localUserExamList = new List<BusinessModel.UserExam> ();
			}

			string _serviceTargetURL = AppConfig.BaseWebserviceURL + "UserExamMapsByUserID/" + aUser.MainSystemID;
			WebClient _service = new WebClient ();
			_service.Headers.Add (HttpRequestHeader.Accept, "application/json");
			string _responseString = _service.DownloadString (_serviceTargetURL);
			if (_responseString != null && _responseString != "") {
				var _jsonResponseArr = (JsonArray)JsonValue.Parse (_responseString);
				List<int> _remoteUserExamIDList = new List<int> ();
				foreach (JsonValue _remoteUserExam in _jsonResponseArr) {
					if (_mainSystemExamIDToLocalExamIDMap.ContainsKey (_remoteUserExam ["ExamID"])) {
						//Only populate user exam that maps to a published/ retired exams, exams that are not published are not good
						BusinessModel.UserExam _userExamToSave = 
							(from x in _localUserExamList where x.MainSystemID == _remoteUserExam ["UserExamMapID"] select x).FirstOrDefault ();
						if(!_userExamToSave.DoSync){
							//Only update if the exam is not marked as needing push to server
							if (_userExamToSave == null) {
								_userExamToSave = new BusinessModel.UserExam ();
								_userExamToSave.MainSystemID = _remoteUserExam ["UserExamMapID"];
								_userExamToSave.DoSync = false;
								_userExamToSave.IsDownloaded = false;
								_userExamToSave.IsCompleted = false;
								_localUserExamList.Add (_userExamToSave);
							}
							_userExamToSave.ExamID = _mainSystemExamIDToLocalExamIDMap [_remoteUserExam ["ExamID"]];
							_userExamToSave.UserID = aUser.UserID;
							_userExamToSave.IsSubmitted = _remoteUserExam ["IsSubmitted"];
							_userExamToSave.IsLearningMode = _remoteUserExam ["IsInteractiveMode"];
							_userExamToSave.HasReadPrivacyPolicy = _remoteUserExam ["HasReadPrivacyPolicy"];
							_userExamToSave.HasReadDisclosure = _remoteUserExam ["HasReadDisclosure"];
							_userExamToSave.SecondsSpent = _remoteUserExam ["SecondsSpent"];
						}
					
					
					}
					_remoteUserExamIDList.Add (_remoteUserExam["UserExamMapID"]);

				}
				//Save all changes in one transaction
				BusinessModel.UserExam.SaveAll (_localUserExamList);

				//Delete all local user exam record that no longer exist in server
				List<BusinessModel.UserExam> _userExamsToDelete;
				_userExamsToDelete = (from x in _localUserExamList where !_remoteUserExamIDList.Contains (x.MainSystemID) select x).ToList ();
				;
				if (_userExamsToDelete.Count > 0) {
					List<string> _deleteSqlList = new List<string> ();
					foreach (BusinessModel.UserExam _userExam in _userExamsToDelete) {
						_deleteSqlList.AddRange (_userExam.GetCascadeDeleteSql ());
					}
					try {
						BusinessModel.SQL.Execute (_deleteSqlList);
					} catch (Exception ex) {
						Console.WriteLine (ex.ToString ());
					}
			
				}
			} 
			return _localUserExamList;
		}

		public static List<BusinessModel.ExamAccess> SyncUserExamAccess (BusinessModel.User aUser)
		{
			//Create the Exam main system id to local id mapping
			Dictionary<int,int> _mainSystemExamIDToLocalExamIDMap = 
				BusinessModel.SQL.ExecuteIntIntDictionary ("SELECT MainSystemID, pkExamID FROM tblExam");

			//Get the local userExamAccess
			List<BusinessModel.ExamAccess> _localExamAccessList =
				BusinessModel.ExamAccess.GetExamAccesssByUserID (aUser.UserID);

			if (_localExamAccessList == null) {
				_localExamAccessList = new List<BusinessModel.ExamAccess> ();
			}

			string _serviceTargetURL = AppConfig.BaseWebserviceURL + "ExamAccessByUserID/" + aUser.MainSystemID;
			WebClient _service = new WebClient ();
			_service.Headers.Add (HttpRequestHeader.Accept, "application/json");
			string _responseString = _service.DownloadString (_serviceTargetURL);
			if (_responseString != null && _responseString != "") {
				var _jsonResponseArr = (JsonArray)JsonValue.Parse (_responseString);
				List<int> _remoteExamAccessLocalExamIDList = new List<int> ();
				foreach (JsonValue _remoteExamAccess in _jsonResponseArr) {
					if (_mainSystemExamIDToLocalExamIDMap.ContainsKey (_remoteExamAccess["ExamID"])) {
						int _localExamIDForAccess = _mainSystemExamIDToLocalExamIDMap [_remoteExamAccess["ExamID"]];
						_remoteExamAccessLocalExamIDList.Add (_localExamIDForAccess);
						BusinessModel.ExamAccess _examAccessToSave = 
							(from x in _localExamAccessList where x.ExamID == _localExamIDForAccess
							 select x).FirstOrDefault ();
						if (_examAccessToSave == null) {
							_examAccessToSave = new BusinessModel.ExamAccess ();
							_examAccessToSave.UserID = aUser.UserID;
							_examAccessToSave.ExamID = _localExamIDForAccess;
							_localExamAccessList.Add (_examAccessToSave);
						}
						_examAccessToSave.HasAccess = _remoteExamAccess ["HasPurchased"];
					}
				}
				//Save the updated list to db
				BusinessModel.ExamAccess.SaveAll (_localExamAccessList);

				//Find out if there is a local exam access record that does not exist in the server
				//If there are such records found, we delete them to keep local same as server
				List<int> _examAccessIDToDeleteList = 
					(from x in _localExamAccessList 
					 where !_remoteExamAccessLocalExamIDList.Contains (x.ExamID) 
					 select x.ExamAccessID).ToList ();
				if (_examAccessIDToDeleteList.Count > 0) {
					BusinessModel.SQL.ExecuteNonQuery (string.Format(
						"DELETE FROM tblExamAccess WHERE pkExamAccessID IN({0})",
						string.Join (",",_examAccessIDToDeleteList)));
				}

			}

			return _localExamAccessList;

		}

		public static bool DownloadExamBaseData (BusinessModel.Exam aExam)
		{
			string _serviceTargetURL = "";
			string _responseString = "";
			WebClient _service = new WebClient ();

			//Since categories are not expected to change in the lifetime of the application, 
			//we will just pull the the category once and resync categories when a category does not exist in the downloaded exam question
			Dictionary<int,int> _mainSystemIDToCategoryIDMap = BusinessModel.SQL.ExecuteIntIntDictionary ("SELECT MainSystemID, pkCategoryID FROM tblCategory");
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
				Console.WriteLine (ex.ToString ());
				return false;
			}
			
			if (_responseString != null && _responseString != "") {
				JsonValue _jsonObj = JsonValue.Parse (_responseString);
				
				// Sync Exam Base Question
				//---------------------------
				List<BusinessModel.Question> _localQuestionList = BusinessModel.Question.GetQuestionsByExamID (aExam.ExamID);
				
				if (_localQuestionList == null) {
					_localQuestionList = new List<BusinessModel.Question> ();
				}
				
				JsonArray _remoteQuestionList = (JsonArray)_jsonObj ["QuestionList"];
				foreach (JsonValue _remoteQuestion in _remoteQuestionList) {
					BusinessModel.Question _matchinglocalQuestion = 
						(from x in _localQuestionList 
						 where x.MainSystemID == _remoteQuestion ["QuestionID"] select x).FirstOrDefault ();
					if (_matchinglocalQuestion == null) {
						_matchinglocalQuestion = new BusinessModel.Question ();
						_matchinglocalQuestion.MainSystemID = _remoteQuestion ["QuestionID"];
						_localQuestionList.Add (_matchinglocalQuestion);
					}
					_matchinglocalQuestion.Stem = _remoteQuestion ["Stem"];
					_matchinglocalQuestion.LeadIn = _remoteQuestion ["LeadIn"];
					_matchinglocalQuestion.Commentary = _remoteQuestion ["Commentary"];
					_matchinglocalQuestion.Reference = _remoteQuestion ["Reference"];

					//Clean up the HTML tags
					//--------------------------

					//Replace 2 or more line break to 2 line breaks
					_matchinglocalQuestion.Stem = Regex.Replace (_matchinglocalQuestion.Stem, "(<br\\s*\\/*>\\s*){2,}", "<br /><br />");
					_matchinglocalQuestion.LeadIn = Regex.Replace (_matchinglocalQuestion.LeadIn, "(<br\\s*\\/*>\\s*){2,}", "<br /><br />");
					_matchinglocalQuestion.Commentary = Regex.Replace (_matchinglocalQuestion.Commentary, "(<br\\s*\\/*>\\s*){2,}", "<br /><br />");
					_matchinglocalQuestion.Reference = Regex.Replace (_matchinglocalQuestion.Reference, "(<br\\s*\\/*>\\s*){2,}", "<br /><br />");

					_matchinglocalQuestion.Stem = Regex.Replace (_matchinglocalQuestion.Stem, "<br\\s*\\/*>", "\n");
					_matchinglocalQuestion.LeadIn = Regex.Replace (_matchinglocalQuestion.LeadIn, "<br\\s*\\/*>", "\n");
					_matchinglocalQuestion.Commentary = Regex.Replace (_matchinglocalQuestion.Commentary, "<br\\s*\\/*>", "\n");
					_matchinglocalQuestion.Reference = Regex.Replace (_matchinglocalQuestion.Reference, "<br\\s*\\/*>", "\n");

					_matchinglocalQuestion.Stem = Regex.Replace (_matchinglocalQuestion.Stem, "<[^>]+>(.+)<\\/[^>]+>","$1");
					_matchinglocalQuestion.LeadIn = Regex.Replace (_matchinglocalQuestion.LeadIn, "<[^>]+>(.+)<\\/[^>]+>","$1");
					_matchinglocalQuestion.Commentary = Regex.Replace (_matchinglocalQuestion.Commentary, "<[^>]+>(.+)<\\/[^>]+>","$1");
					_matchinglocalQuestion.Reference = Regex.Replace (_matchinglocalQuestion.Reference, "<[^>]+>(.+)<\\/[^>]+>","$1");

					_matchinglocalQuestion.ExamID = aExam.ExamID;
					if (_mainSystemIDToCategoryIDMap.ContainsKey (_remoteQuestion ["CategoryID"])) {
						_matchinglocalQuestion.CategoryID = _mainSystemIDToCategoryIDMap [_remoteQuestion ["CategoryID"]];
					} else {
						//If there are categories used by question that does not exist yet, we resync the category
						_mainSystemIDToCategoryIDMap = DownloadCategories ();
						if (_mainSystemIDToCategoryIDMap == null) {
							//If for some reason, the download of categories fail, we cannot continue and so return false for failing to download exam
							return false;
						}
						_matchinglocalQuestion.CategoryID = _mainSystemIDToCategoryIDMap [_remoteQuestion ["CategoryID"]];
					}
					_matchinglocalQuestion.PopulationCorrectPct = _remoteQuestion ["OverallCorrectResponsePct"];
				}
				BusinessModel.Question.SaveAll (_localQuestionList);
				
				
				//After all question is updated, create a mapping between mainsystemid and localquestion id
				Dictionary<int,int> _mainSystemIDToQuestionIDMap = 
					(from x in _localQuestionList 
					 select new{MainSystemID = x.MainSystemID, 
						QuestionID = x.QuestionID}).ToDictionary (y => y.MainSystemID, y => y.QuestionID);
				
				
				// SYNC EXAM BASE ANSWER OPTION
				//-----------------------------------------
				List<BusinessModel.AnswerOption> _localAnswerOptionList = 
					BusinessModel.AnswerOption.GetAnswerOptionsBySQL (string.Format (
						"SELECT tblAnswerOption.* FROM tblAnswerOption INNER JOIN tblQuestion " +
						"ON tblAnswerOption.fkQuestionID = tblQuestion.pkQuestionID " +
						"WHERE tblQuestion.fkExamID={0}", aExam.ExamID));
				
				if (_localAnswerOptionList == null) {
					_localAnswerOptionList = new List<BusinessModel.AnswerOption> ();
				}
				
				JsonArray _remoteAnswerOptionList = (JsonArray)_jsonObj ["AnswerOptionList"];
				foreach (JsonValue _remoteAnswerOption in _remoteAnswerOptionList) {
					BusinessModel.AnswerOption _matchingLocalAnswerOption = 
						(from x in _localAnswerOptionList where x.MainSystemID == _remoteAnswerOption ["AnswerOptionID"] 
						 select x).FirstOrDefault ();
					if (_matchingLocalAnswerOption == null) {
						_matchingLocalAnswerOption = new BusinessModel.AnswerOption ();
						_matchingLocalAnswerOption.MainSystemID = _remoteAnswerOption ["AnswerOptionID"];
						_localAnswerOptionList.Add (_matchingLocalAnswerOption);
					}
					_matchingLocalAnswerOption.QuestionID = _mainSystemIDToQuestionIDMap [_remoteAnswerOption ["QuestionID"]];
					_matchingLocalAnswerOption.AnswerText = _remoteAnswerOption ["Text"];
					_matchingLocalAnswerOption.IsCorrect = _remoteAnswerOption ["IsCorrect"];
				}
				BusinessModel.AnswerOption.SaveAll (_localAnswerOptionList);
				
				// SYNC EXAM BASE IMAGES
				//-----------------------------------------
				List<BusinessModel.Image> _localImageList = 
					BusinessModel.Image.GetImagesBySQL (string.Format (
						"SELECT tblImage.* FROM tblImage INNER JOIN tblQuestion " +
						"ON tblImage.fkQuestionID = tblQuestion.pkQuestionID " +
						"WHERE tblQuestion.fkExamID={0}", aExam.ExamID));
				
				if (_localImageList == null) {
					_localImageList = new List<BusinessModel.Image> ();
				}
				
				JsonArray _remoteImageList = (JsonArray)_jsonObj ["QuestionImageMap"];
				foreach (JsonValue _remoteImage in _remoteImageList) {
					BusinessModel.Image _matchingLocalImage = 
						(from x in _localImageList where x.MainSystemID == _remoteImage ["ImageID"] 
						 select x).FirstOrDefault ();
					if (_matchingLocalImage == null) {
						_matchingLocalImage = new BusinessModel.Image ();
						_matchingLocalImage.MainSystemID = _remoteImage ["ImageID"];
						_matchingLocalImage.DownloadURL = "";
						_matchingLocalImage.FilePath = "";
						_localImageList.Add (_matchingLocalImage);
					}
					_matchingLocalImage.QuestionID = _mainSystemIDToQuestionIDMap [_remoteImage ["QuestionID"]];
					_matchingLocalImage.Title = _remoteImage ["FigureTitle"];
					_matchingLocalImage.ShowInQuestion = _remoteImage ["ShowInQuestion"];
					_matchingLocalImage.ShowInCommentary = _remoteImage ["ShowInCommentary"];
					
				}
				BusinessModel.Image.SaveAll (_localImageList);
				
			} else {
				return false;
			}
			
			return true;
		}

		public delegate void ImageDownloadProgressUpdatedHandler (float downloadedImagePercentage);

		public static bool DownloadExamImageFiles (BusinessModel.Exam aExam, ImageDownloadProgressUpdatedHandler aImageDownloadProgressUpdate)
		{
			string _serviceTargetURL = "";
			string _responseString = "";
			WebClient _service = new WebClient ();
			
			//Get the local image records on db
			List<BusinessModel.Image> _localImageList = 
				BusinessModel.Image.GetImagesBySQL (string.Format (
					"SELECT tblImage.* FROM tblImage INNER JOIN tblQuestion " +
					"ON tblImage.fkQuestionID = tblQuestion.pkQuestionID " +
					"WHERE tblQuestion.fkExamID={0}", aExam.ExamID));
			
			if (_localImageList == null) {
				_localImageList = new List<BusinessModel.Image> ();
			}
			
			_serviceTargetURL = AppConfig.BaseWebserviceURL + string.Format (
				"ImagesByExamID/{0}", aExam.MainSystemID);
			
			try {
				_service.Headers.Add (HttpRequestHeader.Accept, "application/json");
				_responseString = _service.DownloadString (_serviceTargetURL);
			} catch (Exception ex) {
				Console.WriteLine (ex.ToString ());
				return false;
			}
			
			if (_responseString != null && _responseString != "") {
				JsonArray _remoteImageList = (JsonArray)JsonValue.Parse (_responseString);
				//Create dictionary of main system id to remote image obj so we dont have to loop multiple times
				Dictionary<int,JsonValue> _mainSystemIDToRemoteImageObjMap = new Dictionary<int, JsonValue> ();
				foreach (JsonValue _remoteImage in _remoteImageList) {
					_mainSystemIDToRemoteImageObjMap.Add ((int)_remoteImage ["ImageID"], _remoteImage);
				}

				int _imageToDownloadCount = _localImageList.Count;
				int _downloadedImageCount = 0;

				//Loop through the local image list, downloading and saving the downloaded path
				foreach (BusinessModel.Image _localImage in _localImageList) {

					if (_localImage.FilePath == null || _localImage.FilePath == "") {
						//Only perform download if the filepath was empty as it signify that the image is not downloaded yet
						string _remoteRelativeFilePath = _mainSystemIDToRemoteImageObjMap [_localImage.MainSystemID] ["RelativeFilePath"];
						string _downloadURL = string.Format (AppConfig.BaseSiteURL + "ImageHandler.ashx?path={0}", 
						                                     _remoteRelativeFilePath);
						string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
						string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder instead
						//local save path in form of library/ExamImages/{RelativeSavePathOnServer with "/" replaced by "_"}
						string _localSavePath = Path.Combine (libraryPath, "ExamImages", Regex.Replace (_remoteRelativeFilePath, "[/\\\\]+", "_"));
						string _localSaveDirectory = Path.GetDirectoryName (_localSavePath);
						if (!File.Exists (_localSavePath)) {
							
							//Only download if the file has not existed
							try {
								if (!Directory.Exists (_localSaveDirectory)) {
									Directory.CreateDirectory (_localSaveDirectory);
								}
								_service.DownloadFile (_downloadURL, _localSavePath);
							} catch (Exception ex) {
								Console.WriteLine ("Error on downloading image id " + _localImage.ImageID);
								Console.WriteLine (ex.ToString ());
								return false;
							}
						}
						_localImage.DownloadURL = _downloadURL;
						_localImage.FilePath = _localSavePath;
						_localImage.Save ();
					}
					_downloadedImageCount++;

					aImageDownloadProgressUpdate ((float)(_downloadedImageCount/(float)_imageToDownloadCount));
				}
				return true;
			}
			return false;
		}

		public static bool DownloadUserExamCompleteData (BusinessModel.User aUser, BusinessModel.Exam aExam)
		{
			
			string _serviceTargetURL = "";
			string _responseString = "";
			WebClient _service = new WebClient ();
			
			//Download exam base question data (question/image record/ answer option)
			_serviceTargetURL = AppConfig.BaseWebserviceURL + string.Format (
				"GetCompleteUserExamData/{0}/{1}", aUser.MainSystemID, aExam.MainSystemID);
			
			try {
				_service.Headers.Add (HttpRequestHeader.Accept, "application/json");
				_responseString = _service.DownloadString (_serviceTargetURL);
			} catch (Exception ex) {
				Console.WriteLine (ex.ToString ());
				return false;
			}
			
			if (_responseString != null && _responseString != "") {
				JsonValue _userExamCompleteDataObj = JsonValue.Parse (_responseString);
				
				// UPDATE LOCAL USER EXAM RECORD
				//---------------------------------
				BusinessModel.UserExam _localUserExam = 
					BusinessModel.UserExam.GetFirstUserExamByUserIDAndExamID (aUser.UserID, aExam.ExamID);
				JsonValue _remoteUserExam = _userExamCompleteDataObj ["UserExam"];
				if (_localUserExam != null && _localUserExam.MainSystemID != _remoteUserExam ["UserExamMapID"]) {
					//If the userexam is a different user exam than the one downloaded, 
					//the user may have resetted the exam on the web before resynching.
					//Because of this, we will delete the exam
					_localUserExam.CascadeDelete ();
					_localUserExam = new BusinessModel.UserExam ();
					_localUserExam.MainSystemID = _remoteUserExam ["UserExamMapID"];
				} else if (_localUserExam == null) {
					_localUserExam = new BusinessModel.UserExam ();
					_localUserExam.MainSystemID = _remoteUserExam ["UserExamMapID"];
				}
				
				_localUserExam.ExamID = aExam.ExamID;
				_localUserExam.UserID = aUser.UserID;
				_localUserExam.IsSubmitted = _remoteUserExam ["IsSubmitted"];
				_localUserExam.IsLearningMode = _remoteUserExam ["IsInteractiveMode"];
				_localUserExam.HasReadPrivacyPolicy = _remoteUserExam ["HasReadPrivacyPolicy"];
				_localUserExam.HasReadDisclosure = _remoteUserExam ["HasReadDisclosure"];
				_localUserExam.SecondsSpent = _remoteUserExam ["SecondsSpent"];
				_localUserExam.DoSync = false;
				_localUserExam.IsDownloaded = false;
				_localUserExam.IsCompleted = false;
				_localUserExam.Save ();
				
				// GET USER QUESTIONS
				//-------------------------
				List<BusinessModel.UserQuestion> _localUserQuestionList = 
					BusinessModel.UserQuestion.GetUserQuestionsBySQL (
						"SELECT * FROM tblUserQuestion WHERE fkUserExamID=" + _localUserExam.UserExamID);
				if (_localUserQuestionList == null) {
					_localUserQuestionList = new List<BusinessModel.UserQuestion> ();
				}
				
				Dictionary<int, int> _mainSystemToQuestionIDMap = 
					BusinessModel.SQL.ExecuteIntIntDictionary (string.Format (
						"SELECT MainSystemID, pkQuestionID FROM tblQuestion " +
						"WHERE fkExamID={0}", aExam.ExamID));
				
				JsonArray _remoteUserQuestionList = (JsonArray)_userExamCompleteDataObj ["UserQuestionList"];
				foreach (JsonValue _remoteUserQuestion in _remoteUserQuestionList) {
					BusinessModel.UserQuestion _matchingLocalQuestion = 
						(from x in _localUserQuestionList 
						 where x.MainSystemID == _remoteUserQuestion ["UserQuestionID"] select x).FirstOrDefault ();
					if (_matchingLocalQuestion == null) {
						_matchingLocalQuestion = new BusinessModel.UserQuestion ();
						_matchingLocalQuestion.MainSystemID = _remoteUserQuestion ["UserQuestionID"];
						_localUserQuestionList.Add (_matchingLocalQuestion);
					}
					
					_matchingLocalQuestion.QuestionID = _mainSystemToQuestionIDMap [_remoteUserQuestion ["QuestionID"]];
					_matchingLocalQuestion.UserExamID = _localUserExam.UserExamID;
					_matchingLocalQuestion.Sequence = _remoteUserQuestion ["Sequence"];
					if (_remoteUserQuestion ["AnwseredDateTime"] != null && 
						_remoteUserQuestion ["AnwseredDateTime"].ToString () != "" && 
						_remoteUserQuestion ["AnwseredDateTime"].ToString () != "\"\"") {
						_matchingLocalQuestion.AnsweredDateTime = DateTime.Parse (_remoteUserQuestion ["AnwseredDateTime"]);
					} else {
						_matchingLocalQuestion.AnsweredDateTime = null;
					}
					_matchingLocalQuestion.HasAnswered = _remoteUserQuestion ["HasAnswered"];
					_matchingLocalQuestion.HasAnsweredCorrectly = _remoteUserQuestion ["HasAnsweredCorrectly"];
					_matchingLocalQuestion.SecondsSpent = _remoteUserQuestion ["SecondsSpent"];
					_matchingLocalQuestion.DoSync = false;
				}
				
				BusinessModel.UserQuestion.SaveAll (_localUserQuestionList);
				
				Dictionary<int, int> _mainSystemToUserQuestionIDMap = 
					_localUserQuestionList.ToDictionary (x => x.MainSystemID, x => x.UserQuestionID);
				
				
				// GET USER ANSWER OPTIONS
				//---------------------------
				
				Dictionary<int,int> _mainSystemToAnswerOptionIDMap = 
					BusinessModel.SQL.ExecuteIntIntDictionary (string.Format (
						"SELECT tblAnswerOption.MainSystemID, pkAnswerOptionID FROM tblAnswerOption INNER JOIN tblQuestion " +
						"ON tblAnswerOption.fkQuestionID = tblQuestion.pkQuestionID " +
						"WHERE tblQuestion.fkExamID={0}", aExam.ExamID));
				
				List<BusinessModel.UserAnswerOption> _localUserAnswerOptionList = 
					BusinessModel.UserAnswerOption.GetUserAnswerOptionsBySQL (string.Format (
						"SELECT tblUserAnswerOption.* FROM tblUserAnswerOption INNER JOIN tblUserQuestion " +
						"ON tblUserAnswerOption.fkUserQuestionID = tblUserQuestion.pkUserQuestionID " +
						"WHERE tblUserQuestion.fkUserExamID={0}", _localUserExam.UserExamID));
				if (_localUserAnswerOptionList == null) {
					_localUserAnswerOptionList = new List<BusinessModel.UserAnswerOption> ();
				}
				
				JsonArray _remoteUserAnswerOptionList = (JsonArray)_userExamCompleteDataObj ["UserAnswerOptionList"];
				foreach (JsonValue _remoteUserAnswerOption in _remoteUserAnswerOptionList) {
					BusinessModel.UserAnswerOption _matchingLocalUserAnswerOption = 
						(from x in _localUserAnswerOptionList 
						 where x.MainSystemID == _remoteUserAnswerOption ["UserAnswerOptionID"] select x).FirstOrDefault ();
					if (_matchingLocalUserAnswerOption == null) {
						_matchingLocalUserAnswerOption = new BusinessModel.UserAnswerOption ();
						_matchingLocalUserAnswerOption.MainSystemID = _remoteUserAnswerOption ["UserAnswerOptionID"];
						_localUserAnswerOptionList.Add (_matchingLocalUserAnswerOption);
					}
					_matchingLocalUserAnswerOption.UserQuestionID = 
						_mainSystemToUserQuestionIDMap [_remoteUserAnswerOption ["UserQuestionID"]];
					_matchingLocalUserAnswerOption.AnswerOptionID = 
						_mainSystemToAnswerOptionIDMap [_remoteUserAnswerOption ["AnswerOptionID"]];
					_matchingLocalUserAnswerOption.Sequence = _remoteUserAnswerOption ["Sequence"];
					_matchingLocalUserAnswerOption.IsSelected = _remoteUserAnswerOption ["IsUserSelection"];
				}
				BusinessModel.UserAnswerOption.SaveAll (_localUserAnswerOptionList);
				
				return true;
				
			} else {
				return false;
			}
		}
		/// <summary>
		/// Downloads the categories. Returns a dictionary of MainSystemID to local CategoryID or null if the connection fails
		/// </summary>
		/// <returns>A mapping of MainSystemID to the local CategoryID.</returns>
		public static Dictionary<int, int> DownloadCategories ()
		{
			List<BusinessModel.Category> _localCategoryList = BusinessModel.Category.GetAllCategorys ();
			Dictionary<int, BusinessModel.Category> _mainSystemIDToCategoryObjMap = new Dictionary<int, OasisMobile.BusinessModel.Category> ();
			foreach (BusinessModel.Category _localCategory in _localCategoryList) {
				_mainSystemIDToCategoryObjMap.Add (_localCategory.MainSystemID, _localCategory);
			}
			
			//We dont have any categories yet, we will download the categories now.
			string _serviceTargetURL = AppConfig.BaseWebserviceURL + "Categorys";
			string _responseString = "";
			try {
				WebClient _service = new WebClient ();
				_service.Headers.Add (HttpRequestHeader.Accept, "application/json");
				_responseString = _service.DownloadString (_serviceTargetURL);
			} catch (Exception ex) {
				Console.WriteLine (ex.ToString ());
				//Return right away when an exception occurs because that means the get has failed and no data should be returned  
				return null;
			}
			if (_responseString != null && _responseString != "") {
				JsonArray _remoteCategoryList = (JsonArray)JsonValue.Parse (_responseString);
				
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
					
					if (_remoteCategory ["ParentID"] != null && 
						_remoteCategory ["ParentID"].ToString () != "" && 
						_remoteCategory ["ParentID"].ToString () != "\"\"") {
						_matchingLocalCategory.ParentCategoryID = _mainSystemIDToCategoryObjMap [_remoteCategory ["ParentID"]].CategoryID;
					} else {
						_matchingLocalCategory.ParentCategoryID = null;
					}
					_mainSystemIDToLocalIDCategoryMap.Add (_matchingLocalCategory.MainSystemID, _matchingLocalCategory.CategoryID);
				}
				BusinessModel.Category.SaveAll (_localCategoryList);
				
				return _mainSystemIDToLocalIDCategoryMap;
			} else {
				return null;
			}
		}

		public static bool PushAllDoSyncData ()
		{
			Console.WriteLine ("Pushing data to server");
			WebserviceHelper.SyncUserExamPostData _postData = new WebserviceHelper.SyncUserExamPostData ();
			_postData.UserExamList = new List<WebserviceHelper.SyncUserExamPostData.UserExamSyncData> ();
			string _query = "";
			_query = "SELECT tblUserExam.MainSystemID, tblUserExam.HasReadDisclosure, tblUserExam.HasReadPrivacyPolicy FROM tblUserExam " +
					"WHERE DoSync=1";
			_postData.UserExamList = BusinessModel.Repository.Instance.Query<WebserviceHelper.SyncUserExamPostData.UserExamSyncData> (_query);
			if (_postData.UserExamList == null) {
				_postData.UserExamList = new List<WebserviceHelper.SyncUserExamPostData.UserExamSyncData> ();
			}
			List<int> _userExamToUpdateIDList = (from x in _postData.UserExamList select x.MainSystemID).ToList ();

			_query = "SELECT tblUserQuestion.AnsweredDateTime, tblUserQuestion.SecondsSpent, " +
				"tblUserQuestion.MainSystemID AS UserQuestionMainSystemID, " +
				"tblUserAnswerOption.MainSystemID AS SelectedAnswerOptionMainSystemID " +
				"FROM tblUserQuestion LEFT JOIN tblUserAnswerOption " +
				"ON tblUserQuestion.pkUserQuestionID = tblUserAnswerOption.fkUserQuestionID AND tblUserAnswerOption.IsSelected = 1 " +
				"WHERE tblUserQuestion.DoSync=1";
			
			_postData.UserQuestionAnswerPairList = BusinessModel.Repository.Instance.Query<WebserviceHelper.SyncUserExamPostData.UserQuestionAnswerSyncData> (_query);
			if (_postData.UserQuestionAnswerPairList == null) {
				_postData.UserQuestionAnswerPairList = new List<WebserviceHelper.SyncUserExamPostData.UserQuestionAnswerSyncData> ();
			}
			List<int> _userQuestionToUpdateIDList = (from x in _postData.UserQuestionAnswerPairList select x.UserQuestionMainSystemID).ToList ();
			if(_postData.UserExamList.Count>0 || _postData.UserQuestionAnswerPairList.Count>0){
				try {
					WebserviceHelper.SyncUserExamData (_postData);
					BusinessModel.SQL.ExecuteNonQuery (string.Format ("UPDATE tblUserQuestion SET DoSync=0 WHERE MainSystemID IN ({0})",
					                                                  string.Join (",",_userQuestionToUpdateIDList)));
					BusinessModel.SQL.ExecuteNonQuery (string.Format ("UPDATE tblUserExam SET DoSync=0 WHERE MainSystemID IN ({0})",
					                                                  string.Join (",",_userExamToUpdateIDList)));
				} catch (Exception ex) {
					Console.WriteLine (ex.ToString ());
					return false;
				}
			}
			Console.WriteLine ("Data push success");
			return true;

		}

		public static bool SyncUserQuestionAndAnswerFromServer (BusinessModel.User aUser, bool aDoSyncSubmittedExam)
		{
			Console.WriteLine ("Pulling data from server for user: " + aUser.LoginName );
			string _query = "";
			if (aDoSyncSubmittedExam) {
				_query = string.Format ("SELECT * FROM tblUserExam WHERE IsDownloaded=1 AND fkUserID={0}", aUser.UserID);
			} else {
				_query = string.Format ("SELECT * FROM tblUserExam WHERE IsDownloaded=1 AND fkUserID={0} AND IsSubmitted=0", aUser.UserID);
			}

			List<BusinessModel.UserExam> _userExamToSyncList = BusinessModel.UserExam.GetUserExamsBySQL (_query);

			foreach (BusinessModel.UserExam _userExam in _userExamToSyncList) {
				string _response = "";
				try {
					_response = WebserviceHelper.GetRemoteUserQuestionDataByUserExamID (_userExam.MainSystemID);
				} catch (Exception ex) {
					Console.WriteLine (ex.ToString ());
					return false;

				}
				if (_response == null || _response == "") {
					continue;
				}

				JsonArray _remoteUserQuestionList = (JsonArray)JsonValue.Parse (_response);
				List<BusinessModel.UserQuestion> _localUserQuestionList = BusinessModel.UserQuestion.GetUserQuestionsBySQL ("SELECT * FROM tblUserQuestion WHERE fkUserExamID=" + _userExam.UserExamID);
				List<string> _answerOptionUpdateQueryList = new List<String> ();
				foreach (JsonValue _remoteUserQuestion in _remoteUserQuestionList) {
					BusinessModel.UserQuestion _matchingLocalUserQuestion = (from x in _localUserQuestionList where x.MainSystemID == _remoteUserQuestion ["UserQuestionID"] select x).FirstOrDefault ();
					if (!_matchingLocalUserQuestion.DoSync) {
						if (_remoteUserQuestion ["AnwseredDateTime"] != null &&
							_remoteUserQuestion ["AnwseredDateTime"].ToString () != "" && 
							_remoteUserQuestion ["AnwseredDateTime"].ToString () != "\"\"") {
							DateTime _remoteUserQuestionAnsweredDate = DateTime.Parse (_remoteUserQuestion["AnwseredDateTime"]);
							if (_matchingLocalUserQuestion.AnsweredDateTime == null || _matchingLocalUserQuestion.AnsweredDateTime != _remoteUserQuestionAnsweredDate) {
								//Since the answered date time does not match, we will check if the answer has changed
								try {
									string _answerOptionResponse = WebserviceHelper.GetRemoteUserAnswerOptionDataByUserQuestionID (_matchingLocalUserQuestion.MainSystemID);
									JsonArray _remoteAnswerOptionList = (JsonArray)JsonValue.Parse (_answerOptionResponse);
									int _selectedAnswerOptionMainSystemID = -1;
									foreach (JsonValue _remoteAnswerOption in _remoteAnswerOptionList) {
										if(_remoteAnswerOption["IsUserSelection"]){
											_selectedAnswerOptionMainSystemID = _remoteAnswerOption["UserAnswerOptionID"];
										}
									}
									if(_selectedAnswerOptionMainSystemID>0){
										_answerOptionUpdateQueryList.Add ("UPDATE tblUserAnswerOption SET IsSelected=0 WHERE fkUserQuestionID=" + _matchingLocalUserQuestion.UserQuestionID); 
										_answerOptionUpdateQueryList.Add ("UPDATE tblUserAnswerOption SET IsSelected=1 WHERE MainSystemID=" + _selectedAnswerOptionMainSystemID); 
									}else{
										Console.WriteLine ("Error on updating remote question id: " +_matchingLocalUserQuestion.MainSystemID + ". AnswerDateTime exist but no answer is selected");
									}

								} catch (Exception ex) {
									Console.WriteLine (ex.ToString ());
									return false;
								}
								_matchingLocalUserQuestion.AnsweredDateTime = _remoteUserQuestionAnsweredDate;
							}
						
						} else {
							if (_matchingLocalUserQuestion.AnsweredDateTime != null) {
								_matchingLocalUserQuestion.AnsweredDateTime = null;
								//Since there are no answered date time, we should also unselect the answer
								_answerOptionUpdateQueryList.Add ("UPDATE tblUserAnswerOption SET IsSelected=0 WHERE fkUserQuestionID=" + _matchingLocalUserQuestion.UserQuestionID);
							}

						}


						_matchingLocalUserQuestion.HasAnswered = _remoteUserQuestion ["HasAnswered"];
						_matchingLocalUserQuestion.HasAnsweredCorrectly = _remoteUserQuestion ["HasAnsweredCorrectly"];
						_matchingLocalUserQuestion.SecondsSpent = _remoteUserQuestion ["SecondsSpent"];
					}
				}

				BusinessModel.SQL.Execute (_answerOptionUpdateQueryList);
				BusinessModel.UserQuestion.SaveAll (_localUserQuestionList);
			}
			Console.WriteLine ("Pull Success");
			return true;



		}
	}
}

