// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using Xamarin.UITest;
using System.Reflection;

namespace MyDriving.UITests
{
    public class AppInitializer
    {
		const string appPath = "../../../MyDriving.iOS/bin/iPhoneSimulator/Release/MyDrivingiOS.app";
        public static string apkPath;

		private static IApp app;
		public static IApp App
		{
			get
			{
				if (app == null)
					throw new NullReferenceException("'AppInitializer.App' not set. Call 'AppInitializer.StartApp(platform)' before trying to access it.");
				return app;
			}
		}

        public static IApp StartApp(Platform platform)
        {
			if (platform == Platform.Android)
			{
				string currentFile = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
				FileInfo fi = new FileInfo(currentFile);
				string dir = fi.Directory.Parent.Parent.Parent.FullName;

				// PathToAPK is a property or an instance variable in the test class
				apkPath = Path.Combine(dir, "MyDriving.Android", "bin", "XTC", "com.microsoft.mydriving-Signed.apk");

				app = ConfigureApp
					.Android
                    .ApkFile(apkPath)
                    .StartApp(Xamarin.UITest.Configuration.AppDataMode.Clear);
			}
			else
			{
				app = ConfigureApp
					.iOS
					//.AppBundle(appPath)
                    .InstalledApp("com.microsoft.mydriving")
					.StartApp(Xamarin.UITest.Configuration.AppDataMode.Clear);
			}

			return app;
        }
    }
}