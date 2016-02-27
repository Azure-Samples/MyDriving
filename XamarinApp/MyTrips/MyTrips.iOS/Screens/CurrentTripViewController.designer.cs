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
    [Register ("CurrentTripViewController")]
    partial class CurrentTripViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblCost { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDistance { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDuration { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblGallons { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblMpg { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton recordButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MyTrips.iOS.TripMapView tripMapView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider tripSlider { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton wayPointA { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton wayPointB { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblCost != null) {
                lblCost.Dispose ();
                lblCost = null;
            }

            if (lblDistance != null) {
                lblDistance.Dispose ();
                lblDistance = null;
            }

            if (lblDuration != null) {
                lblDuration.Dispose ();
                lblDuration = null;
            }

            if (lblGallons != null) {
                lblGallons.Dispose ();
                lblGallons = null;
            }

            if (lblMpg != null) {
                lblMpg.Dispose ();
                lblMpg = null;
            }

            if (recordButton != null) {
                recordButton.Dispose ();
                recordButton = null;
            }

            if (tripMapView != null) {
                tripMapView.Dispose ();
                tripMapView = null;
            }

            if (tripSlider != null) {
                tripSlider.Dispose ();
                tripSlider = null;
            }

            if (wayPointA != null) {
                wayPointA.Dispose ();
                wayPointA = null;
            }

            if (wayPointB != null) {
                wayPointB.Dispose ();
                wayPointB = null;
            }
        }
    }
}