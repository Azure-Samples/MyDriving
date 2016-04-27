// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using UIKit;

namespace MyDriving.iOS
{
    public class ThemeManager
    {
        public static void ApplyTheme()
        {
            // Status Bar
            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;

            //NavigationBar
            UINavigationBar.Appearance.BarTintColor = "0087D2".ToUIColor();
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
            // [[UITabBar appearance] setTintColor:[UIColor redColor]];
            UITabBar.Appearance.TintColor = "0087D2".ToUIColor();
            UITabBarItem.Appearance.SetTitleTextAttributes(
                new UITextAttributes {Font = UIFont.FromName("Avenir-Book", 10f)}, UIControlState.Normal);
        }
    }
}