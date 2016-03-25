// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Foundation;
using System;
using UIKit;

namespace MyDriving.iOS
{
	public partial class GettingStartedViewController : UIPageViewController
	{
		GettingStartedContentViewController pageOne = (GettingStartedContentViewController)UIStoryboard.FromName("Main", null).InstantiateViewController("gettingStartedContentViewController");

		public GettingStartedViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			AutomaticallyAdjustsScrollViewInsets = false;
			//self.automaticallyAdjustsScrollViewInsets = false;

			Title = "MyDriving";
			NavigationItem.SetRightBarButtonItem(new UIBarButtonItem("Dismiss", UIBarButtonItemStyle.Plain, (sender, e) =>
			{
				var viewController = UIStoryboard.FromName("Main", null).InstantiateViewController("loginViewController");
				var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
				appDelegate.Window.RootViewController = viewController;
			}), false);

			pageOne.PageIndex = 0;
			pageOne.Image = UIImage.FromBundle("screen_1.png");

			SetViewControllers(new UIViewController[] { pageOne }, UIPageViewControllerNavigationDirection.Forward, true, null);
			DataSource = new PageViewControllerSource();
		}

		public class PageViewControllerSource : UIPageViewControllerDataSource
		{
			public override nint GetPresentationCount(UIPageViewController pageViewController)
			{
				return 5;
			}

			public override UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
			{
				var vc = (GettingStartedContentViewController)referenceViewController;
				var index = vc.PageIndex;

				if (index == 0)
					return null;

				return GettingStartedContentViewController.ControllerForPageIndex(index-1);
			}

			public override UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
			{
				var vc = (GettingStartedContentViewController)referenceViewController;
				var index = vc.PageIndex;

				if (index == 4)
					return null;

				return GettingStartedContentViewController.ControllerForPageIndex(index+1);
			}
		}
    }
}
 