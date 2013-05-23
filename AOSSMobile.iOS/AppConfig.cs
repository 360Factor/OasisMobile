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

			public static class iPhone
			{
				public const string LoginBackgroundImage="Images/ClothBG-iPhone 5.jpg";
			}

			public static class iPad
			{
				public const string LoginBackgroundImage="Images/ClothBG-iPad@2x.jpg";
			}
		}

	}
}

