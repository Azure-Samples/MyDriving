// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using UIKit;

namespace MyDriving.iOS
{
    public partial class TripSummaryViewController : UIViewController
    {
        public TripSummaryViewController(IntPtr handle) : base(handle)
        {
        }

        public ViewModel.CurrentTripViewModel ViewModel { get; set; }

        public override void ViewDidLoad()
        {
            lblDateTime.Text = $"{DateTime.Now.ToString("M")}  {DateTime.Now.ToString("t")}";
            lblDistance.Text = ViewModel.TripSummary.TotalDistanceDisplay;
            lblDuration.Text = ViewModel.TripSummary.TotalTimeDisplay;
            lblFuelConsumed.Text = ViewModel.TripSummary.FuelDisplay;
            lblTopSpeed.Text = ViewModel.TripSummary.MaxSpeedDisplay;

            lblDistance.Alpha = 0;
            lblDuration.Alpha = 0;
            lblTopSpeed.Alpha = 0;
            lblFuelConsumed.Alpha = 0;
            lblTopSpeed.Alpha = 0;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            lblDistance.FadeIn(0.4, 0.1f);
            lblDuration.FadeIn(0.4, 0.2f);
            lblTopSpeed.FadeIn(0.4, 0.3f);
            lblFuelConsumed.FadeIn(0.4, 0.4f);
            lblTopSpeed.FadeIn(0.4, 0.5f);
        }

        async partial void BtnClose_TouchUpInside(UIButton sender)
        {
            await DismissViewControllerAsync(true);
        }
    }
}