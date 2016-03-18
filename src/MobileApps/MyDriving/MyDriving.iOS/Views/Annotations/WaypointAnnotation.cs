// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using CoreLocation;

namespace MyDriving.iOS
{
    public class WaypointAnnotation : BaseCustomAnnotation
    {
        public WaypointAnnotation(CLLocationCoordinate2D annotationLocation, string waypoint) : base(annotationLocation)
        {
            Waypoint = waypoint;
        }

        public string Waypoint { get; set; }
    }
}