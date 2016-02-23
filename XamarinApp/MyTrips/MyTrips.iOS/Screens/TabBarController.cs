using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MyTrips.iOS
{
	partial class TabBarController : UITabBarController
	{
		public TabBarController (IntPtr handle) : base (handle)
		{
		}

		async public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			var authenticated = false;

			//TODO Check if the user is authenticated. 
			if (authenticated == false)
			{
				var viewController = Storyboard.InstantiateViewController("loginViewController");
				if (viewController == null)
					return;

				await PresentViewControllerAsync(viewController, false);
			}
		}
	}
}
