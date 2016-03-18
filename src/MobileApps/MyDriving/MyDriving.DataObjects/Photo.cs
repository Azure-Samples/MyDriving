// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace MyDriving.DataObjects
{
    public class Photo : BaseDataObject
    {
        public string TripId { get; set; }

        public string PhotoUrl { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}