// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

#if !BACKEND
using Newtonsoft.Json;
using MyDriving.Utils;

#endif

namespace MyDriving.DataObjects
{
    public class TripPoint : BaseDataObject
    {
        public string TripId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        /// <summary>
        ///     Gets or sets the speed, in km/h
        /// </summary>
        /// <value>The speed.</value>
        public double Speed { get; set; }

        public DateTime RecordedTimeStamp { get; set; }

        /// <summary>
        ///     Gets or sets the sequence order number starting at 0
        /// </summary>
        /// <value>The sequence.</value>
        public int Sequence { get; set; }

        public double RPM { get; set; }

        public double ShortTermFuelBank { get; set; }

        public double LongTermFuelBank { get; set; }

        public double ThrottlePosition { get; set; }

        public double RelativeThrottlePosition { get; set; }

        public double Runtime { get; set; }

        public double DistanceWithMalfunctionLight { get; set; }

        public double EngineLoad { get; set; }

        public double MassFlowRate { get; set; }

        public double EngineFuelRate { get; set; }

        public string VIN { get; set; }

        public bool HasOBDData { get; set; }

        public bool HasSimulatedOBDData { get; set; }
    }
}