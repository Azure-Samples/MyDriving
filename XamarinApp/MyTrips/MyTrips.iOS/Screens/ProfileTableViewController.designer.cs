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

namespace MyTrips.iOS
{
    [Register ("ProfileTableViewController")]
    partial class ProfileTableViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblBetterThan { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDrivingSkills { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblPercentage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MyTrips.iOS.CustomControls.CirclePercentage PercentageView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblBetterThan != null) {
                lblBetterThan.Dispose ();
                lblBetterThan = null;
            }

            if (lblDrivingSkills != null) {
                lblDrivingSkills.Dispose ();
                lblDrivingSkills = null;
            }

            if (lblPercentage != null) {
                lblPercentage.Dispose ();
                lblPercentage = null;
            }

            if (PercentageView != null) {
                PercentageView.Dispose ();
                PercentageView = null;
            }
        }
    }
}