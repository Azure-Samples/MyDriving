using System;
using System.IO;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace MyTrips.UITests
{
    public class AppInitializer
    {
		// const string apkPath = "../../../com.xamarin.samples.taskydroid-Signed.apk";
		// const string appPath = "../../../TaskyiOS.app";
		// const string ipaBundleId = "com.xamarin.samples.taskytouch";

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
				app = ConfigureApp
					.Android
					// .ApkFile(apkPath)
					.StartApp();
			}
			else
			{
				app = ConfigureApp
					.iOS
					// .AppBundle(appPath)
					// .InstalledApp(ipaBundleId)
					.StartApp();
			}

			return app;
        }
    }
}