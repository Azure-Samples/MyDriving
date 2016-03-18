// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using CoreLocation;
using MyDriving.DataObjects;

namespace MyDriving.iOS
{
    public class PoiAnnotation : BaseCustomAnnotation
    {
        public PoiAnnotation(POI pointOfInterest, CLLocationCoordinate2D coordinate) : base(coordinate)
        {
            PointOfInterest = pointOfInterest;
        }

        public POI PointOfInterest { get; set; }

        public string Description
        {
            get
            {
                switch (PointOfInterest.POIType)
                {
                    case POIType.HardAcceleration:
                        return "Hard Acceleration";
                    case POIType.HardBrake:
                        return "Hard Brake";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}