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
    [Register ("CurrentTripViewController")]
    partial class CurrentTripViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel endTimeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelFourTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelFourValue { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelOneTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelOneValue { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelThreeTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelThreeValue { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelTwoTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelTwoValue { get; set; }

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
        UIKit.UIView tripInfoView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MyDriving.iOS.TripMapView tripMapView { get; set; }

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

            if (labelFourTitle != null) {
                labelFourTitle.Dispose ();
                labelFourTitle = null;
            }

            if (labelFourValue != null) {
                labelFourValue.Dispose ();
                labelFourValue = null;
            }

            if (labelOneTitle != null) {
                labelOneTitle.Dispose ();
                labelOneTitle = null;
            }

            if (labelOneValue != null) {
                labelOneValue.Dispose ();
                labelOneValue = null;
            }

            if (labelThreeTitle != null) {
                labelThreeTitle.Dispose ();
                labelThreeTitle = null;
            }

            if (labelThreeValue != null) {
                labelThreeValue.Dispose ();
                labelThreeValue = null;
            }

            if (labelTwoTitle != null) {
                labelTwoTitle.Dispose ();
                labelTwoTitle = null;
            }

            if (labelTwoValue != null) {
                labelTwoValue.Dispose ();
                labelTwoValue = null;
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