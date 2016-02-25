using System;
using UIKit;

namespace MyTrips.iOS
{
	public class ThemeManager
	{
		static UIColor PRIMARY = "607DB8".ToColor ();
		static UIColor PRIMARY_DARK = "111820".ToColor ();
	    static UIColor PRIMARY_LIGHT = "CFD8DC".ToColor ();
		static UIColor ACCENT = "FF4081".ToColor ();
		static UIColor PRIMARY_TEXT = "212121".ToColor ();
		static UIColor SECONDARY_TEXT = "727272".ToColor ();
		static UIColor ICONS = "FFFFFF".ToColor ();
		static UIColor DIVIDER = "B6B6B6".ToColor ();

		public static void ApplyTheme ()
		{
			
			// Status Bar
			UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;

			// Navigation Bar
			UINavigationBar.Appearance.BarTintColor = PRIMARY_DARK;
			UINavigationBar.Appearance.SetTitleTextAttributes (new UITextAttributes {
				Font = UIFont.FromName("AvenirNext-Medium", 17),
				TextColor = PRIMARY
			});

			UITabBarItem.Appearance.SetTitleTextAttributes (new UITextAttributes {
				Font = UIFont.FromName("AvenirNext", 15),
				TextColor = PRIMARY.Lighten(5)
			}, UIControlState.Normal);

			//UITabBar.Appearance.TintColor = PRIMARY;

		}
	}
}