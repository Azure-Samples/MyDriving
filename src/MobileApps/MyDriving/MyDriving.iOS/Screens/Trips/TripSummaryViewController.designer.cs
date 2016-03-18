// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MyDriving.iOS
{
    [Register ("TripSummaryViewController")]
    partial class TripSummaryViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnClose { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDateTime { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDistance { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDuration { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblFuelConsumed { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTopSpeed { get; set; }

        [Action ("BtnClose_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnClose_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnClose != null) {
                btnClose.Dispose ();
                btnClose = null;
            }

            if (lblDateTime != null) {
                lblDateTime.Dispose ();
                lblDateTime = null;
            }

            if (lblDistance != null) {
                lblDistance.Dispose ();
                lblDistance = null;
            }

            if (lblDuration != null) {
                lblDuration.Dispose ();
                lblDuration = null;
            }

            if (lblFuelConsumed != null) {
                lblFuelConsumed.Dispose ();
                lblFuelConsumed = null;
            }

            if (lblTopSpeed != null) {
                lblTopSpeed.Dispose ();
                lblTopSpeed = null;
            }
        }
    }
}