using System;
using System.Collections.Generic;
using System.Net;
using System.Json;
using System.Linq;

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
				BusinessModel.SQL.ExecuteIntIntDictionary ("SELECT MainSystemID, pkExamID FROM Exam");

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
					if(_mainSystemExamIDToLocalExamIDMap.ContainsKey (_remoteUserExam ["ExamID"])){
						//Only populate user exam that maps to a published/ retired exams, exams that are not published are not good
						BusinessModel.UserExam _userExamToSave = 
							(from x in _localUserExamList where x.MainSystemID == _remoteUserExam ["UserExamMapID"] select x).FirstOrDefault ();
						if (_userExamToSave == null) {
							_userExamToSave = new BusinessModel.UserExam ();
							_userExamToSave.MainSystemID = _remoteUserExam ["UserExamMapID"];
							_userExamToSave.DoSync = false;
							_userExamToSave.IsDownloaded = false;
							_userExamToSave.IsCompleted = false;
							_localUserExamList.Add (_userExamToSave);
						}
						_userExamToSave.ExamID = _mainSystemExamIDToLocalExamIDMap[_remoteUserExam ["ExamID"]];
						_userExamToSave.UserID = aUser.UserID;
						_userExamToSave.IsSubmitted = _remoteUserExam ["IsSubmitted"];
						_userExamToSave.IsLearningMode = _remoteUserExam ["IsInteractiveMode"];
						_userExamToSave.HasReadPrivacyPolicy = _remoteUserExam ["HasReadPrivacyPolicy"];
						_userExamToSave.HasReadDisclosure = _remoteUserExam ["HasReadDisclosure"];
						_userExamToSave.SecondsSpent = _remoteUserExam ["SecondsSpent"];
					
					}
					_remoteUserExamIDList.Add (_remoteUserExam["UserExamMapID"]);

				}
				//Save all changes in one transaction
				BusinessModel.UserExam.SaveAll(_localUserExamList);

				//Delete all local user exam record that no longer exist in server
				List<BusinessModel.UserExam> _userExamsToDelete;
				_userExamsToDelete = (from x in _localUserExamList where !_remoteUserExamIDList.Contains (x.MainSystemID) select x).ToList ();;
				if(_userExamsToDelete.Count>0){
					List<string> _deleteSqlList = new List<string>();
					foreach (BusinessModel.UserExam _userExam in _userExamsToDelete) {
						_deleteSqlList.AddRange (_userExam.GetCascadeDeleteSql ());
					}
					try{
						BusinessModel.SQL.Execute (_deleteSqlList);
					}
					catch(Exception ex){
						Console.WriteLine (ex.ToString ());
					}
			
				}
			} 
			return _localUserExamList;
		}

		public static List<BusinessModel.ExamAccess> SyncUserExamAccess(BusinessModel.User aUser){
			//Create the Exam main system id to local id mapping
			Dictionary<int,int> _mainSystemExamIDToLocalExamIDMap = 
				BusinessModel.SQL.ExecuteIntIntDictionary ("SELECT MainSystemID, pkExamID FROM Exam");

			//Get the local userExamAccess
			List<BusinessModel.ExamAccess> _localExamAccessList =
				BusinessModel.ExamAccess.GetExamAccesssByUserID (aUser.UserID);

			if(_localExamAccessList == null){
				_localExamAccessList =  new List<BusinessModel.ExamAccess>();
			}

			string _serviceTargetURL = AppConfig.BaseWebserviceURL + "ExamAccessByUserID/" + aUser.MainSystemID;
			WebClient _service = new WebClient ();
			_service.Headers.Add (HttpRequestHeader.Accept, "application/json");
			string _responseString = _service.DownloadString (_serviceTargetURL);
			if (_responseString != null && _responseString != "") {
				var _jsonResponseArr = (JsonArray)JsonValue.Parse (_responseString);
				List<int> _remoteExamAccessLocalExamIDList = new List<int>();
				foreach (JsonValue _remoteExamAccess in _jsonResponseArr) {
					if(_mainSystemExamIDToLocalExamIDMap.ContainsKey (_remoteExamAccess["ExamID"])){
						int _localExamIDForAccess = _mainSystemExamIDToLocalExamIDMap[_remoteExamAccess["ExamID"]];
						_remoteExamAccessLocalExamIDList.Add (_localExamIDForAccess);
						BusinessModel.ExamAccess _examAccessToSave = 
							(from x in _localExamAccessList where x.ExamID == _localExamIDForAccess
							 select x).FirstOrDefault ();
						if(_examAccessToSave == null){
							_examAccessToSave = new BusinessModel.ExamAccess();
							_examAccessToSave.UserID = aUser.UserID;
							_examAccessToSave.ExamID = _localExamIDForAccess;
							_localExamAccessList.Add (_examAccessToSave);
						}
						_examAccessToSave.HasAccess = _remoteExamAccess["HasPurchased"];
					}
				}
				//Save the updated list to db
				BusinessModel.ExamAccess.SaveAll (_localExamAccessList);

				//Find out if there is a local exam access record that does not exist in the server
				//If there are such records found, we delete them to keep local same as server
				List<int> _examAccessIDToDeleteList = 
					(from x in _localExamAccessList 
					 where !_remoteExamAccessLocalExamIDList.Contains(x.ExamID) 
					 select x.ExamAccessID).ToList();
				if(_examAccessIDToDeleteList.Count>0){
					BusinessModel.SQL.ExecuteNonQuery (string.Format(
						"DELETE FROM ExamAccess WHERE pkExamAccessID IN({0})",
						string.Join (",",_examAccessIDToDeleteList)));
				}

			}

			return _localExamAccessList;

		}

	}

}

