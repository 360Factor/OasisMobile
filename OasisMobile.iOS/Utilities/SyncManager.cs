using System;
using System.Collections.Generic;
using System.Net;
using System.Json;
using System.Linq;

namespace OasisMobile.iOS
{
	public static class SyncManager
	{
		public static List<BussinessLogicLayer.Exam> SyncExamDataFromServer ()
		{
			//Get local exams
			List<BussinessLogicLayer.Exam> _localExamList = BussinessLogicLayer.Exam.GetAllExams ();
			if (_localExamList == null) {
				_localExamList = new List<BussinessLogicLayer.Exam> ();
			}

			//Get published remote exam
			string _serviceTargetURL = AppConfig.BaseWebserviceURL + "ExamsIsPublished";
			WebClient _service = new WebClient ();
			_service.Headers.Add (HttpRequestHeader.Accept, "application/json");
			string _responseString = _service.DownloadString (_serviceTargetURL);
			if (_responseString != null && _responseString != "") {
				var _jsonResponseArr = (JsonArray)JsonValue.Parse (_responseString);

				foreach (JsonValue _remoteExam in _jsonResponseArr) {
					BussinessLogicLayer.Exam _examToSave = 
						(from x in _localExamList where x.MainSystemID == _remoteExam ["ExamID"] select x).FirstOrDefault ();
					if (_examToSave == null) {
						_examToSave = new BussinessLogicLayer.Exam ();
						_examToSave.MainSystemID = _remoteExam ["ExamID"];
						_localExamList.Add (_examToSave);
					}
					_examToSave.ExamName = _remoteExam ["Name"];
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
			_responseString = _service.DownloadString (_serviceTargetURL);
			if (_responseString != null && _responseString != "") {
				var _jsonResponseArr = (JsonArray)JsonValue.Parse (_responseString);
				
				foreach (JsonValue _remoteExam in _jsonResponseArr) {
					BussinessLogicLayer.Exam _examToSave = 
						(from x in _localExamList where x.MainSystemID == _remoteExam ["ExamID"] select x).FirstOrDefault ();
					if (_examToSave == null) {
						_examToSave = new BussinessLogicLayer.Exam ();
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
			BussinessLogicLayer.Exam.SaveAll (_localExamList);

			return _localExamList;
		}

		public static List<BussinessLogicLayer.UserExam> SyncUserExamDataFromServer (BussinessLogicLayer.User aUser)
		{
			//Create the Exam main system id to local id mapping
			List<BussinessLogicLayer.Exam> _localExamList = BussinessLogicLayer.Exam.GetAllExams();
			Dictionary<int,int> _mainSystemExamIDToLocalExamIDMap = new Dictionary<int,int>();
			foreach (BussinessLogicLayer.Exam _exam in _localExamList){
				_mainSystemExamIDToLocalExamIDMap.Add (_exam.MainSystemID,_exam.ExamID);
			}

			//Get Local User Exams
			List<BussinessLogicLayer.UserExam> _localUserExamList = BussinessLogicLayer.UserExam.GetUserExamsByUserID (aUser.UserID);
			if (_localUserExamList == null) {
				_localUserExamList = new List<BussinessLogicLayer.UserExam> ();
			}

			string _serviceTargetURL = AppConfig.BaseWebserviceURL + "UserExamMapsByUserID/" + aUser.MainSystemID;
			WebClient _service = new WebClient ();
			_service.Headers.Add (HttpRequestHeader.Accept, "application/json");
			string _responseString = _service.DownloadString (_serviceTargetURL);
			if (_responseString != null && _responseString != "") {
				var _jsonResponseArr = (JsonArray)JsonValue.Parse (_responseString);
				foreach (JsonValue _remoteUserExam in _jsonResponseArr) {
					BussinessLogicLayer.UserExam _userExamToSave = 
						(from x in _localUserExamList where x.MainSystemID == _remoteUserExam ["UserExamMapID"] select x).FirstOrDefault ();
					if (_userExamToSave == null) {
						_userExamToSave = new BussinessLogicLayer.UserExam ();
						_userExamToSave.MainSystemID = _remoteUserExam ["UserExamMapID"];
						_userExamToSave.DoSync = false;
						_userExamToSave.IsDownloaded = false;
					}
					_userExamToSave.ExamID = _mainSystemExamIDToLocalExamIDMap[_remoteUserExam ["ExamID"]];
					_userExamToSave.UserID = _remoteUserExam ["UserID"];
					_userExamToSave.IsSubmitted = _remoteUserExam ["IsSubmitted"];
					_userExamToSave.IsLearningMode = _remoteUserExam ["IsLearningMode"];
					_userExamToSave.HasReadPrivacyPolicy = _remoteUserExam ["HasReadPrivacyPolicy"];
					_userExamToSave.HasReadDisclosure = _remoteUserExam ["HasReadDisclosure"];
					_userExamToSave.SecondsSpent = _remoteUserExam ["SecondsSpent"];
				}
				BussinessLogicLayer.UserExam.SaveAll(_localUserExamList);
			} 
			return _localUserExamList;
		}


	}
}

