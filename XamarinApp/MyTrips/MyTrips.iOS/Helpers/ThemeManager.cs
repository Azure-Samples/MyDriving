using System;
using UIKit;

namespace MyTrips.iOS
{
	public class ThemeManager
	{
		public static void ApplyTheme ()
		{
			
			// Status Bar
			UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;

			// Navigation Bar
			UINavigationBar.Appearance.BarTintColor = Colors.PRIMARY_DARK;
			UINavigationBar.Appearance.SetTitleTextAttributes (new UITextAttributes {
				Font = UIFont.FromName("AvenirNext-Medium", 17),
				TextColor = Colors.PRIMARY
			});

			UITabBarItem.Appearance.SetTitleTextAttributes (new UITextAttributes {
				Font = UIFont.FromName("AvenirNext", 15),
				TextColor = Colors.PRIMARY.Lighten(5)
			}, UIControlState.Normal);

			//UITabBar.Appearance.TintColor = PRIMARY;

		}
	}
}