using System;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace OasisMobile.iOS
{
	public static class WebserviceHelper
	{
		private class GenerateUserExamRequestData
		{

			public int ExamID{ get; set; }

			public int UserID{ get; set; }

			public GenerateUserExamRequestData ()
			{

			}
		}

		public class SyncUserExamPostData
		{
			public SyncUserExamPostData(){

			}

			public List<UserExamSyncData> UserExamList {get; set;}
			public List<UserQuestionAnswerSyncData> UserQuestionAnswerPairList { get; set; }

			public class UserExamSyncData
			{
				public bool HasReadDisclosure { get; set;}
				public bool HasReadPrivacyPolicy { get; set;}
				public int MainSystemID { get; set;}

				public UserExamSyncData(){

				}

			}

			public class UserQuestionAnswerSyncData
			{
				public DateTime AnsweredDateTime{ get; set;}
				public int SecondsSpent{ get; set;}
				public int? SelectedAnswerOptionMainSystemID { get; set;}
				public int UserQuestionMainSystemID { get; set;}

				public UserQuestionAnswerSyncData(){

				}

			}
		}

		public static string GenerateUserExam (bool aIsLearningMode, int aRemoteExamID, int aRemoteUserID)
		{
			WebClient _service = new WebClient ();
			string _postURL;
			if (aIsLearningMode) {
				_postURL = AppConfig.BaseWebserviceURL + "GenerateLearningModeExam";
			} else {
				_postURL = AppConfig.BaseWebserviceURL + "GenerateExaminationModeExam";
			}
			_service.Headers.Add (HttpRequestHeader.Accept, "application/json"); 
			_service.Headers.Add (HttpRequestHeader.ContentType, "application/json"); 

			GenerateUserExamRequestData _postObj = new GenerateUserExamRequestData () {
				ExamID = aRemoteExamID,
				UserID = aRemoteUserID
			};

			string _postJSONString = JsonConvert.SerializeObject (_postObj);
			string _response = _service.UploadString (_postURL, _postJSONString);
			return _response;

		}

		public static string SyncUserExamData (SyncUserExamPostData aPostData)
		{
			WebClient _service = new WebClient ();
			string _postURL = AppConfig.BaseWebserviceURL + "SyncUserExamData";
			_service.Headers.Add (HttpRequestHeader.Accept, "application/json"); 
			_service.Headers.Add (HttpRequestHeader.ContentType, "application/json"); 

			string _postJSONString = JsonConvert.SerializeObject (aPostData);
			string _response = _service.UploadString (_postURL, _postJSONString);
			return _response;
		}

		public static string GetRemoteUserQuestionDataByUserExamID(int aRemoteUserExamID){
			WebClient _service = new WebClient ();
			string _serviceURL = AppConfig.BaseWebserviceURL + "UserQuestionsByUserExamMapID/" + aRemoteUserExamID;
			_service.Headers.Add (HttpRequestHeader.Accept, "application/json"); 
			string _response = _service.DownloadString (_serviceURL);
			return _response;
		}

		public static string GetRemoteUserAnswerOptionDataByUserQuestionID(int aRemoteUserQuestionID){
			WebClient _service = new WebClient ();
			string _serviceURL = AppConfig.BaseWebserviceURL + "UserAnswerOptionsByUserQuestionID/" + aRemoteUserQuestionID;
			_service.Headers.Add (HttpRequestHeader.Accept, "application/json"); 
			string _response = _service.DownloadString (_serviceURL);
			return _response;
		}

	}
}

