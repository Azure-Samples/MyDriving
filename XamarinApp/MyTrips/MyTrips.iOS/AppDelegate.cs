using System;
using System.Threading.Tasks;

using Foundation;
using UIKit;

using MyTrips.DataStore.Abstractions;
using MyTrips.Interfaces;
using MyTrips.iOS.Helpers;
using MyTrips.Shared;
using MyTrips.Utils;
using MyTrips.Utils.Interfaces;

using HockeyApp;

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
			ServiceLocator.Instance.Add<ILogger, PlatformLogger>();
			ServiceLocator.Instance.Add<IHubIOT, IOTHub>();
			ServiceLocator.Instance.Add<IOBDDevice, OBDDevice>();

			Xamarin.Insights.Initialize(Logger.InsightsKey);

#if XTC
            Xamarin.Calabash.Start();
#endif

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
			SQLitePCL.CurrentPlatform.Init();
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

			if (!Settings.Current.IsLoggedIn)
			{
				var viewController = UIStoryboard.FromName("Main", null).InstantiateViewController("loginViewController"); // Storyboard.InstantiateViewController("loginViewController");
				Window.RootViewController = viewController;
			}
			else
			{
				var tabBarController = Window.RootViewController as UITabBarController;
				tabBarController.SelectedIndex = 1;
			}

			#if DEBUG
			Xamarin.Calabash.Start();
			#endif

			return true;
		}

		public override void WillEnterForeground(UIApplication application)
		{
			var tabBarController = Window.RootViewController as UITabBarController;
			var navigationController = tabBarController.ViewControllers[1] as UINavigationController;
			var currentTripViewController = navigationController.TopViewController as CurrentTripViewController;
		}

		#region Background Refresh
		private const double MINIMUM_BACKGROUND_FETCH_INTERVAL = 900;
		private void SetMinimumBackgroundFetchInterval()
		{
			UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(MINIMUM_BACKGROUND_FETCH_INTERVAL);
		}

		public override async void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
		{
			var downloadSuccessful = false;
			try
			{
				var manager = ServiceLocator.Instance.Resolve<IStoreManager>() as DataStore.Azure.StoreManager;
				if (manager != null)
				{
					await manager.SyncAllAsync(true);
					downloadSuccessful = true;
				}

			}
			catch (Exception ex)
			{
				Logger.Instance.Report(ex);
			}

			if (downloadSuccessful)
				completionHandler(UIBackgroundFetchResult.NewData);
			else
				completionHandler(UIBackgroundFetchResult.Failed);
		}
		#endregion
	}

    [Register("TripApplication")]
    public class TripApplication : UIApplication
    {
        public override void MotionBegan(UIEventSubtype motion, UIEvent evt)
        {
            if (motion == UIEventSubtype.MotionShake)
                BITHockeyManager.SharedHockeyManager.FeedbackManager.ShowFeedbackComposeViewWithGeneratedScreenshot();
        }
    }
}