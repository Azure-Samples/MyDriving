// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using UIKit;
using CoreGraphics;

namespace MyDriving.iOS
{
    partial class TabBarController : UITabBarController
    {
        public TabBarController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupTabChangeAnimation();
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

                UIView.Animate(0.2,
                    () => { toView.Frame = new CGRect(0, destFrame.Y, destFrame.Width, destFrame.Height); }, () =>
                    {
                        fromView.RemoveFromSuperview();
                        SelectedViewController = controller;
                    });
                return true;
            };
        }
    }
}