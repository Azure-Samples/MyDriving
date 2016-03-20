// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace MyDriving.DataObjects
{
    public class POI : BaseDataObject
    {
        public string TripId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public POIType POIType { get; set; }
        public DateTime Timestamp { get; set; }
    }
}