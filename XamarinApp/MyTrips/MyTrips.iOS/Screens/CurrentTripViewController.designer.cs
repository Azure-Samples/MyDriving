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
        MyTrips.iOS.TripMapView tripMapView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (tripMapView != null) {
                tripMapView.Dispose ();
                tripMapView = null;
            }
        }
    }
}