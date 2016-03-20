// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using CoreLocation;
using MapKit;

namespace MyDriving.iOS
{
    public class BaseCustomAnnotation : MKAnnotation
    {
        readonly CLLocationCoordinate2D coordinate;

        public BaseCustomAnnotation(CLLocationCoordinate2D annotationLocation)
        {
            coordinate = annotationLocation;
        }

        public override CLLocationCoordinate2D Coordinate => coordinate;
    }
}