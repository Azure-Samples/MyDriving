// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Foundation;
using System;
using UIKit;

namespace MyDriving.iOS
{
    public partial class GettingStartedContentViewController : UIViewController
    {
		public UIImage Image { get; set; }
		public int PageIndex { get; set; }

		public GettingStartedContentViewController (IntPtr handle) : base (handle)
        {
        }

		public static GettingStartedContentViewController ControllerForPageIndex(int pageIndex)
		{
			var imagePath = string.Format($"screen_{pageIndex+1}.png");
			var image = UIImage.FromBundle(imagePath);
			var page = (GettingStartedContentViewController)UIStoryboard.FromName("Main", null).InstantiateViewController("gettingStartedContentViewController");
			page.Image = image;
			page.PageIndex = pageIndex;

			return page;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("background_started.png"));
			imageView.Image = Image;
		}
	}
}
 