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
			UINavigationBar.Appearance.TintColor = UIColor.White;
			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes
			{
				Font = UIFont.FromName("AvenirNext-Medium", 17),
				TextColor = UIColor.White
			});

			UIBarButtonItem.Appearance.SetTitleTextAttributes (new UITextAttributes {
				Font = UIFont.FromName("AvenirNext-Medium", 15),
				TextColor = UIColor.White,

			}, UIControlState.Normal);


			UITabBarItem.Appearance.SetTitleTextAttributes(new UITextAttributes
			{
				Font = UIFont.FromName("AvenirNext", 15),
				TextColor = UIColor.White,
			}, UIControlState.Normal);



			UITabBar.Appearance.TintColor = UIColor.White;
			UITabBar.Appearance.SelectedImageTintColor = "4571AF".ToUIColor();
		}
	}
}