// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using CoreLocation;
using UIKit;

namespace MyDriving.iOS
{
    public class CarAnnotation : BaseCustomAnnotation
    {
        public CarAnnotation(CLLocationCoordinate2D annotationLocation, UIColor color) : base(annotationLocation)
        {
            Color = color;
        }

        public UIColor Color { get; set; }
    }
}