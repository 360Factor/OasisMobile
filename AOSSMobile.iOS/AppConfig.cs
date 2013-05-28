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

		public static class ImagePaths
		{
			public const string ClientLogo = "Images/OasisLogo560px.png";
			public const string FlyoutNavBackgroundImage = "Images/Background-Blueprint.jpg";

			public static class iPhone
			{
				public const string LoginBackgroundImage="Images/AOSSM_BG-iPhone.png";
				public const string LoginBackgroundImage_568h="Images/AOSSM_BG-iPhone568h.png";
			}

			public static class iPad
			{
				public const string LoginBackgroundImage_Potrait="Images/AOSSM_BG-iPad.png";
				public const string LoginBackgroundImage_Landscape="Images/AOSSM_BG-iPad-Landscape.png";
			}
		}

	}
}

