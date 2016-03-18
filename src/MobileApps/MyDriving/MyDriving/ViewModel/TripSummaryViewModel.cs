// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using MyDriving.Utils;
using System;

namespace MyDriving.ViewModel
{
    public class TripSummaryViewModel : ViewModelBase
    {
        double _fuelUsed;

        long _hardAccelerations;


        long _hardStops;

        double _maxSpeed;

        double _totalDistance;

        double _totalTime;
        public string FuelUnits => Settings.MetricUnits ? "L" : "gal.";

        public double FuelConverted => Settings.MetricUnits ? FuelUsed/.264172 : FuelUsed;

        public string FuelDisplayNoUnits => FuelConverted.ToString("F");

        public string FuelDisplay => $"{FuelDisplayNoUnits} {FuelUnits}";

        public string DistanceUnits => Settings.MetricDistance ? "km" : "miles";

        public string TotalDistanceDisplayNoUnits => DistanceConverted.ToString("F");

        public string TotalDistanceDisplay => $"{TotalDistanceDisplayNoUnits} {DistanceUnits}";

        public double DistanceConverted => (Settings.Current.MetricDistance ? (TotalDistance*1.60934) : TotalDistance);

        public string SpeedUnits => Settings.MetricDistance ? "km/h" : "mph";

        public double MaxSpeedConverted => Settings.MetricDistance ? MaxSpeed : MaxSpeed*0.621371;

        public string MaxSpeedDisplayNoUnits => MaxSpeedConverted.ToString("F");

        public string MaxSpeedDisplay => $"{MaxSpeedDisplayNoUnits} {SpeedUnits}";

        public DateTime Date { get; set; }

        public string TotalTimeDisplay
        {
            get
            {
                var time = TimeSpan.FromSeconds(TotalTime);
                if (time.TotalMinutes < 1)
                    return $"{time.Seconds}s";

                if (time.TotalHours < 1)
                    return $"{time.Minutes}m {time.Seconds}s";

                return $"{(int) time.TotalHours}h {time.Minutes}m {time.Seconds}s";
            }
        }

        public double TotalDistance
        {
            get { return _totalDistance; }
            set
            {
                if (!SetProperty(ref _totalDistance, value))
                    return;

                OnPropertyChanged(nameof(DistanceUnits));
                OnPropertyChanged(nameof(TotalDistanceDisplay));
                OnPropertyChanged(nameof(TotalDistanceDisplayNoUnits));
                OnPropertyChanged(nameof(DistanceConverted));
            }
        }

        public double FuelUsed
        {
            get { return _fuelUsed; }
            set
            {
                if (!SetProperty(ref _fuelUsed, value))
                    return;

                OnPropertyChanged(nameof(FuelUnits));
                OnPropertyChanged(nameof(FuelDisplay));
                OnPropertyChanged(nameof(FuelDisplayNoUnits));
                OnPropertyChanged(nameof(FuelConverted));
            }
        }

        public double TotalTime
        {
            get { return _totalTime; }
            set
            {
                if (!SetProperty(ref _totalTime, value))
                    return;

                OnPropertyChanged(nameof(TotalTimeDisplay));
            }
        }

        public double MaxSpeed
        {
            get { return _maxSpeed; }
            set
            {
                if (!SetProperty(ref _maxSpeed, value))
                    return;

                OnPropertyChanged(nameof(SpeedUnits));
                OnPropertyChanged(nameof(MaxSpeedConverted));
                OnPropertyChanged(nameof(MaxSpeedDisplayNoUnits));
                OnPropertyChanged(nameof(MaxSpeedDisplay));
            }
        }

        public long HardStops
        {
            get { return _hardStops; }
            set { SetProperty(ref _hardStops, value); }
        }

        public long HardAccelerations
        {
            get { return _hardAccelerations; }
            set { SetProperty(ref _hardAccelerations, value); }
        }
    }
}