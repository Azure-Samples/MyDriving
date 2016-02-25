using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using CoreGraphics;

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
			SetupTabChangeAnimation();

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

		void SetupTabChangeAnimation()
		{
			ShouldSelectViewController = (tabController, controller) =>
			{
				if (SelectedViewController == null || controller == SelectedViewController)
					return true;

				var fromView = SelectedViewController.View;
				var toView = controller.View;

				var destFrame = fromView.Frame;
				const float offset = 25;

				//Position toView off screen
				fromView.Superview.AddSubview(toView);
				toView.Frame = new CGRect(offset, destFrame.Y, destFrame.Width, destFrame.Height);

				UIView.Animate(0.1,
				               () =>
				{
					toView.Frame = new CGRect(0, destFrame.Y, destFrame.Width, destFrame.Height);
				}, () =>
				{
					fromView.RemoveFromSuperview();
					SelectedViewController = controller;
				});
				return true;
			};
		}
	}
}
