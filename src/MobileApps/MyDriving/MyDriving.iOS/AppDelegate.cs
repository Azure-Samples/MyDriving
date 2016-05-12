// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using MyDriving.DataStore.Abstractions;
using MyDriving.Interfaces;
using MyDriving.iOS.Helpers;
using MyDriving.Shared;
using MyDriving.Utils;
using MyDriving.Utils.Interfaces;
using HockeyApp;

namespace MyDriving.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window { get; set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            ThemeManager.ApplyTheme();
            ViewModel.ViewModelBase.Init();
            application.IdleTimerDisabled = true;

            ServiceLocator.Instance.Add<IAuthentication, Authentication>();
            ServiceLocator.Instance.Add<ILogger, PlatformLogger>();
            ServiceLocator.Instance.Add<IOBDDevice, OBDDevice>();

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            SQLitePCL.CurrentPlatform.Init();

#if !XTC
            if (!string.IsNullOrWhiteSpace(Logger.HockeyAppiOS))
            {
                var manager = BITHockeyManager.SharedHockeyManager;
                manager.Configure(Logger.HockeyAppiOS);
                manager.StartManager();
            }
#endif

            if (!Settings.Current.IsLoggedIn)
            {
#if XTC
		var viewController = UIStoryboard.FromName("Main", null)
		                                 .InstantiateViewController("loginViewController");
		Window.RootViewController = viewController;
#else
                if (Settings.Current.FirstRun)
                {
                    var viewController = UIStoryboard.FromName("Main", null)
                                 .InstantiateViewController("gettingStartedViewController");
                    var navigationController = new UINavigationController(viewController);
                    Window.RootViewController = navigationController;

                    Settings.Current.FirstRun = false;
                }
                else
                {
                    var viewController = UIStoryboard.FromName("Main", null)
                                                     .InstantiateViewController("loginViewController");
                    Window.RootViewController = viewController;
                }
#endif
            }
            else
            {
                //When the first scre app is launched after user has logged in, initialize the processor that manages connection to OBD Device and to the IOT Hub
                MyDriving.Services.OBDDataProcessor.GetProcessor().Initialize(ViewModel.ViewModelBase.StoreManager);

                var tabBarController = Window.RootViewController as UITabBarController;
                tabBarController.SelectedIndex = 1;
            }

#if XTC
            Xamarin.Calabash.Start();
#endif

            return true;
        }
        #region Background Refresh

        private const double MinimumBackgroundFetchInterval = 900;

        private void SetMinimumBackgroundFetchInterval()
        {
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(MinimumBackgroundFetchInterval);
        }

        public override async void PerformFetch(UIApplication application,
            Action<UIBackgroundFetchResult> completionHandler)
        {
            try
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
            catch (Exception ex)
            {

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
                BITHockeyManager.SharedHockeyManager.FeedbackManager.ShowFeedbackComposeViewWithGeneratedScreenshot();
        }
    }
}
