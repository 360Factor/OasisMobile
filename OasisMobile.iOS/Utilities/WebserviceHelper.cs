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

		public static string GenerateUserExam (bool aIsLearningMode, int aExamID, int aUserID)
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

			GenerateUserExamRequestData _postObj = new GenerateUserExamRequestData (){ExamID = aExamID, UserID = aUserID};

			string _postJSONString = JsonConvert.SerializeObject (_postObj);
			string _response = _service.UploadString (_postURL, _postJSONString);

			return _response ;

		}

	


	}
}

