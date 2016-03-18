using System;
using System.IO;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using System.Reflection;

namespace MyDriving.UITests
{
    public class AppInitializer
    {
		//const string apkPath = "../../../com.xamarin.samples.taskydroid-Signed.apk";
		const string appPath = "../../../MyDriving.iOS/bin/iPhoneSimulator/Debug/MyDrivingiOS.app";

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
				var PathToAPK = Path.Combine(dir, "MyTrips.Android", "bin", "XTC", "com.microsoft.mytrips-Signed.apk");

				Console.WriteLine (PathToAPK);

				app = ConfigureApp
					.Android
					.ApkFile(PathToAPK)
					.StartApp();
			}
			else
			{
				app = ConfigureApp
					.iOS
					.AppBundle(appPath)
					.StartApp(Xamarin.UITest.Configuration.AppDataMode.Clear);
			}

			return app;
        }
    }
}