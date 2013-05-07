using System;
using System.Collections.Generic;

namespace OasisMobile.BusinessModel
{
	public partial class ExamAccess
	{
		
		public static BusinessModel.ExamAccess GetFirstExamAccessByUserIDAndExamID(int aUserID, int aExamID){
			return BusinessModel.ExamAccess .GetFirstExamAccessBySQL (string.Format(
				"SELECT * FROM ExamAccess " +
				"WHERE fkUserID={0} AND fkExamID={1} LIMIT 1", aUserID, aExamID));
		}
		
		
		
	}
	
}

