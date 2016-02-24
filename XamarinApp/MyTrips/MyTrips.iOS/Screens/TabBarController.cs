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

		bool authenticated = false;

		async public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);


			//TODO Check if the user is authenticated. 
			if (authenticated == false)
			{
				var viewController = Storyboard.InstantiateViewController("loginViewController");
				if (viewController == null)
					return;

				await PresentViewControllerAsync(viewController, false);

				//TODO We shouldn't manually set this but I've done it whilst we wait for other things to be put in place. Once the backend is working and we can actually auth, we should remove this.
				authenticated = true; 
			}
		}
	}
}
