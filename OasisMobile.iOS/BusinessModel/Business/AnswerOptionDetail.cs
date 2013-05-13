using System;
using System.Collections.Generic;

namespace OasisMobile.BusinessModel
{
	public class UserAnswerOptionDetail
	{
		public int UserAnswerOptionID { get; set; }

		public int AnswerOptionID { get; set; }

		public int UserQuestionID { get; set; }

		public int Sequence { get; set; }

		public bool IsSelected { get; set; }

		public bool IsCorrect { get; set; }

		public string AnswerOptionText { get; set; }

		public UserAnswerOptionDetail ()
		{
		
		}

		public static List<UserAnswerOptionDetail> GetUserAnswerOptionDetailListByUserQuestionID(int aUserQuestionID){
			string _query = string.Format ("SELECT UserAnswerOption.pkUserAnswerOptionID AS UserAnswerOptionID, " +
			                "UserAnswerOption.fkAnswerOptionID AS AnswerOptionID, UserAnswerOption.fkUserQuestionID AS UserQuestionID, " +
			                "UserAnswerOption.Sequence, UserAnswerOption.IsSelected, AnswerOption.IsCorrect, AnswerOption.AnswerText AS AnswerOptionText " +
							"FROM UserAnswerOption INNER JOIN AnswerOption " +
							"ON UserAnswerOption.fkAnswerOptionID = AnswerOption.pkAnswerOptionID " +
							"WHERE fkUserQuestionID={0}",aUserQuestionID);
			return Repository.Instance.Query<UserAnswerOptionDetail>(_query);
		}
	}
}

