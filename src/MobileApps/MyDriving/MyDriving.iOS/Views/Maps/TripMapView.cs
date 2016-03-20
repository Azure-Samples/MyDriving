// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using CoreLocation;
using MapKit;

namespace MyDriving.iOS
{
    public partial class TripMapView : MKMapView
    {
        public TripMapView(IntPtr handle) : base(handle)
        {
        }

        public void DrawRoute(CLLocationCoordinate2D[] route)
        {
            AddOverlay(MKPolyline.FromCoordinates(route));
        }
    }
}