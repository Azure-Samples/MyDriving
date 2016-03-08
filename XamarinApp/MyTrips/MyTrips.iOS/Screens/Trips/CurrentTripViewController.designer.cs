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
        UIKit.UILabel endTimeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblConsumption { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDistance { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDistanceUnits { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDuration { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTemperature { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton recordButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView sliderView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel startTimeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem takePhotoButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView tripInfoView { get; set; }

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
            if (endTimeLabel != null) {
                endTimeLabel.Dispose ();
                endTimeLabel = null;
            }

            if (lblConsumption != null) {
                lblConsumption.Dispose ();
                lblConsumption = null;
            }

            if (lblDistance != null) {
                lblDistance.Dispose ();
                lblDistance = null;
            }

            if (lblDistanceUnits != null) {
                lblDistanceUnits.Dispose ();
                lblDistanceUnits = null;
            }

            if (lblDuration != null) {
                lblDuration.Dispose ();
                lblDuration = null;
            }

            if (lblTemperature != null) {
                lblTemperature.Dispose ();
                lblTemperature = null;
            }

            if (recordButton != null) {
                recordButton.Dispose ();
                recordButton = null;
            }

            if (sliderView != null) {
                sliderView.Dispose ();
                sliderView = null;
            }

            if (startTimeLabel != null) {
                startTimeLabel.Dispose ();
                startTimeLabel = null;
            }

            if (takePhotoButton != null) {
                takePhotoButton.Dispose ();
                takePhotoButton = null;
            }

            if (tripInfoView != null) {
                tripInfoView.Dispose ();
                tripInfoView = null;
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