using System;
using System.Threading.Tasks;

using CoreSpotlight;
using Foundation;
using UIKit;

using MyTrips.DataStore.Abstractions;
using MyTrips.Utils;
using MyTrips.Interfaces;
using MyTrips.iOS.Helpers;
using MyTrips.ViewModel;

using HockeyApp;
using MyTrips.Shared;

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
            ServiceLocator.Instance.Add<MyTrips.Utils.Interfaces.ILogger, MyTrips.Shared.PlatformLogger>();
            ServiceLocator.Instance.Add<IHubIOT, IOTHub>();
            
            //TODO: Need to add #debug compile dir for all offline\mock interfaces
            ServiceLocator.Instance.Add<IOBDDevice, OBDDevice>();
            //ServiceLocator.Instance.Add<IOBDDevice, OBDDeviceSim>();

            Xamarin.Insights.Initialize(Logger.InsightsKey);
			
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

			return true;
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
		#region CoreSpotlight Search
		public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
		{
			if (userActivity.ActivityType == CSSearchableItem.ActionType)
			{
				var uuid = userActivity.UserInfo.ObjectForKey(CSSearchableItem.ActivityIdentifier);

				if (uuid == null)
					return true;

				var appDelegate = (AppDelegate)application.Delegate;
				var tabBarController = (UITabBarController) appDelegate.Window.RootViewController;
				tabBarController.SelectedIndex = 0;
				var navigationController = (UINavigationController)tabBarController.ViewControllers[0];
				var tripsViewController = (TripsTableViewController) navigationController.TopViewController;
				tripsViewController.NavigationController.PopToRootViewController(false);

				var trip = tripsViewController.ViewModel.Trips[Int32.Parse(uuid.ToString())];

				var currentTripVc = UIStoryboard.FromName("Main", null).InstantiateViewController("CURRENT_TRIP_STORYBIARD_IDENTIFIER") as CurrentTripViewController;
				currentTripVc.PastTripsDetailViewModel = new PastTripsDetailViewModel(trip);
				tripsViewController.NavigationController.PushViewController(currentTripVc, false);
			}

			return true;
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