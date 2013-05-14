using System;

namespace OasisMobile.iOS
{
	public static class AppConfig
	{
		public static string BaseWebserviceURL{
			get{
				return "http://oasis-ws.lightwellsolution.com/";
			}
		}

		public static string BaseSiteURL{
			get{
				return "http://oasis.lightwellsolution.com/";
			}
		}

		public static Boolean IsProduction{
			get{
				return true;
			}
		}

		public static Boolean AllowExamPurchase{
			get{
				return true;
			}
		}

	}
}

