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
    [Register ("ProfileTableViewController")]
    partial class ProfileTableViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgAvatar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDrivingSkills { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblScore { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MyDriving.iOS.CustomControls.CirclePercentage PercentageView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (imgAvatar != null) {
                imgAvatar.Dispose ();
                imgAvatar = null;
            }

            if (lblDrivingSkills != null) {
                lblDrivingSkills.Dispose ();
                lblDrivingSkills = null;
            }

            if (lblScore != null) {
                lblScore.Dispose ();
                lblScore = null;
            }

            if (PercentageView != null) {
                PercentageView.Dispose ();
                PercentageView = null;
            }
        }
    }
}