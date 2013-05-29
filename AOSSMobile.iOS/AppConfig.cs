using System;
using MonoTouch.UIKit;

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
				public const string LoginBackgroundImage="Images/AOSSM_BG-iPhone.png";
				public const string LoginBackgroundImage_568h="Images/AOSSM_BG-iPhone568h.png";
			}

			public static class iPad
			{
				public const string LoginBackgroundImage_Potrait="Images/AOSSM_BG-iPad.png";
				public const string LoginBackgroundImage_Landscape="Images/AOSSM_BG-iPad-Landscape.png";
			}
		}

		public static class FlyoutNavTheme{
			public const string BackgroundImagePath = "Images/Background-Blue.jpg";
			public const string ExamIconPath = "Images/Icon-Book.png";
			public const string SettingIconPath = "Images/Icon-Gear.png";
			public const string SupportIconPath = "Images/Icon-Lifebuoy.png";
			public const string AboutIconPath = "Images/Icon-Beaker.png";
			public static UIColor TextColor{
				get{
					return UIColor.FromRGB (210,210,210);
				}
			}
			public static UIColor SeparatorColor{
				get{
					return UIColor.FromRGB (200, 200, 200);
				}
			}
		}

	}
}

