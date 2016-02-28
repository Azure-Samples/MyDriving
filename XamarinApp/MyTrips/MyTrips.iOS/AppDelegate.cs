using System;
using System.Threading.Tasks;

using Foundation;
using UIKit;

using MyTrips.Utils;
using MyTrips.Interfaces;
using MyTrips.iOS.Helpers;

using HockeyApp;
using MyTrips.DataStore.Abstractions;

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
            Xamarin.Insights.Initialize(Logger.InsightsKey);
			if (!string.IsNullOrWhiteSpace(Logger.HockeyAppiOS))
			{
				Setup.EnableCustomCrashReporting(() =>
					{
						var manager = BITHockeyManager.SharedHockeyManager;
						manager.Configure(Logger.HockeyAppiOS);
						manager.StartManager();
						manager.Authenticator.AuthenticateInstallation();
						AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
							Setup.ThrowExceptionAsNative(e.ExceptionObject);
						TaskScheduler.UnobservedTaskException += (sender, e) =>
							Setup.ThrowExceptionAsNative(e.Exception);
					});
			}

			return true;
		}

		#region Background Refresh

		// Minimum number of seconds between a background refresh
		// 15 minutes = 15 * 60 = 900 seconds
		private const double MINIMUM_BACKGROUND_FETCH_INTERVAL = 900;

		private void SetMinimumBackgroundFetchInterval()
		{
			UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(MINIMUM_BACKGROUND_FETCH_INTERVAL);
		}

		// Called whenever your app performs a background fetch
		public override async void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
		{
			// Do Background Fetch
			var downloadSuccessful = false;
			try
			{
				// Download data
				var manager = ServiceLocator.Instance.Resolve<IStoreManager>() as DataStore.Azure.StoreManager;
				if (manager != null)
				{
					await manager.SyncAllAsync(true);
					downloadSuccessful = true;
				}

			}
			catch (Exception ex)
			{
				//TODO probably send this to hockeyApp. 
			}

			// If you don't call this, your application will be terminated by the OS.
			// Allows OS to collect stats like data cost and power consumption
			if (downloadSuccessful)
			{
				completionHandler(UIBackgroundFetchResult.NewData);
			}
			else {
				completionHandler(UIBackgroundFetchResult.Failed);
			}
		}

		#endregion
	}

    [Register("TripApplication")]
    public class TripApplication : UIApplication
    {
        public override void MotionBegan(UIEventSubtype motion, UIEvent evt)
        {
            if (motion == UIEventSubtype.MotionShake)
            {
                BITHockeyManager.SharedHockeyManager.FeedbackManager.ShowFeedbackComposeViewWithGeneratedScreenshot();
            }
        }
    }
}
