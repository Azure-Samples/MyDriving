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

			//NavigationBar
			UINavigationBar.Appearance.BarTintColor = Colors.BLUE;
			UINavigationBar.Appearance.TintColor = UIColor.White;

			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes
			{
				Font = UIFont.FromName("Avenir-Medium", 17f),
				TextColor = UIColor.White
			});

			//NavigationBar Buttons 
			UIBarButtonItem.Appearance.SetTitleTextAttributes(new UITextAttributes
			{
				Font = UIFont.FromName("Avenir-Medium", 17f),
				TextColor = UIColor.White,
			}, UIControlState.Normal);

			//TabBar
			UITabBarItem.Appearance.SetTitleTextAttributes(new UITextAttributes{ Font = UIFont.FromName("Avenir-Book", 10f) }, UIControlState.Normal);
		}
	}
}