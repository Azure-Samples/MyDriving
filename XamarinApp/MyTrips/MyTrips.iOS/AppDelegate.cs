using Foundation;
using UIKit;

namespace MyTrips.iOS
{
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			ThemeManager.ApplyTheme ();
			ViewModel.ViewModelBase.Init();

			return true;
		}
	}
}


