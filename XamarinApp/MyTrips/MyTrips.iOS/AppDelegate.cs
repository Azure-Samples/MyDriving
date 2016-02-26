using System;
using System.Threading.Tasks;

using Foundation;
using UIKit;

using MyTrips.Utils;
using MyTrips.Interfaces;
using MyTrips.iOS.Helpers;

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
