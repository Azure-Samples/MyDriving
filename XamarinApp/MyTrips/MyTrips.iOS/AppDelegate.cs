using Foundation;
using UIKit;
using MyTrips.Utils;
using MyTrips.Interfaces;
using MyTrips.iOS.Helpers;
using HockeyApp;
using System;
using System.Threading.Tasks;

using Plugin.Geolocator;

namespace MyTrips.iOS
{
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			ThemeManager.ApplyTheme();
			ViewModel.ViewModelBase.Init();

			ServiceLocator.Instance.Add<IAuthentication, Authentication>();

			if (!string.IsNullOrWhiteSpace(Logger.HockeyAppKey))
			{
				Setup.EnableCustomCrashReporting(() =>
					{

						//Get the shared instance
						var manager = BITHockeyManager.SharedHockeyManager;

						//Configure it to use our APP_ID
						manager.Configure(Logger.HockeyAppKey);

						//Start the manager
						manager.StartManager();

						//Authenticate (there are other authentication options)
						manager.Authenticator.AuthenticateInstallation();

						//Rethrow any unhandled .NET exceptions as native iOS 
						// exceptions so the stack traces appear nicely in HockeyApp
						AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
							Setup.ThrowExceptionAsNative(e.ExceptionObject);

						TaskScheduler.UnobservedTaskException += (sender, e) =>
							Setup.ThrowExceptionAsNative(e.Exception);
					});
			}

			return true;
		}
	}

	[Register("TripApplication")]
	public class TripApplication : UIApplication
	{
		public override void MotionBegan(UIEventSubtype motion, UIEvent evt)
		{
			//TODO Implement shake
		}
	}
}
