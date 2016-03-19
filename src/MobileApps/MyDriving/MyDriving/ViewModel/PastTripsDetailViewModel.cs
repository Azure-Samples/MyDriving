// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MyDriving.Helpers;
using MyDriving.Utils;
using MyDriving.DataObjects;

namespace MyDriving.ViewModel
{
    public class PastTripsDetailViewModel : ViewModelBase
    {
        string distance = "0.0";

        string distanceUnits = "miles";

        string elapsedTime = "0s";

        string fuelConsumption = "N/A";

        string fuelConsumptionUnits = "gal";

        ICommand loadTripCommand;

        TripPoint position;

        string speed = "0.0";

        string speedUnits = "Mph";

        public PastTripsDetailViewModel()
        {
            FuelConsumptionUnits = Settings.MetricUnits ? "Liters" : "Gallons";
            DistanceUnits = Settings.MetricDistance ? "Kilometers" : "Miles";
        }

        public PastTripsDetailViewModel(Trip trip) : this()
        {
            Title = trip.Name;
            //make sure the points are ordered
            trip.Points = trip.Points.OrderBy(p => p.Sequence).ToArray();
            Trip = trip;
        }

        public Trip Trip { get; set; }

        public TripPoint CurrentPosition
        {
            get { return position; }
            set
            {
                SetProperty(ref position, value);
                UpdateTripInformationForPoint();
            }
        }

        public string ElapsedTime
        {
            get { return elapsedTime; }
            set { SetProperty(ref elapsedTime, value); }
        }

        public string Distance
        {
            get { return distance; }
            set { SetProperty(ref distance, value); }
        }

        public string DistanceUnits
        {
            get { return distanceUnits; }
            set { SetProperty(ref distanceUnits, value); }
        }

        public string FuelConsumption
        {
            get { return fuelConsumption; }
            set { SetProperty(ref fuelConsumption, value); }
        }

        public string FuelConsumptionUnits
        {
            get { return fuelConsumptionUnits; }
            set { SetProperty(ref fuelConsumptionUnits, value); }
        }

        public string Speed
        {
            get { return speed; }
            set { SetProperty(ref speed, value); }
        }

        public string SpeedUnits
        {
            get { return speedUnits; }
            set { SetProperty(ref speedUnits, value); }
        }

        public ICommand LoadTripCommand =>
            loadTripCommand ??
            (loadTripCommand = new RelayCommand<string>(async id => await ExecuteLoadTripCommandAsync(id)));

        public async Task ExecuteLoadTripCommandAsync(string id)
        {
            if (IsBusy)
                return;

            var progress = Acr.UserDialogs.UserDialogs.Instance.Loading("Loading trip details...",
                maskType: Acr.UserDialogs.MaskType.Clear);

            try
            {
                IsBusy = true;

                Trip = await StoreManager.TripStore.GetItemAsync(id);
                Title = Trip.Name;
                for (int i = 0; i < Trip.Points.Count; i++)
                {
                    var point = Trip.Points[i];
                    if (point.MassFlowRate == -255)
                    {
                        point.MassFlowRate = i == 0 ? 0 : Trip.Points[i - 1].MassFlowRate;
                    }
                    if (point.Speed == -255)
                    {
                        point.Speed = i == 0 ? 0 : Trip.Points[i - 1].Speed;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }
            finally
            {
                progress?.Dispose();
                IsBusy = false;
            }
        }

        public void UpdateTripInformationForPoint()
        {
            var timeDif = position.RecordedTimeStamp - Trip.RecordedTimeStamp;

            //track seconds, minutes, then hours
            if (timeDif.TotalMinutes < 1)
                ElapsedTime = $"{timeDif.Seconds}s";
            else if (timeDif.TotalHours < 1)
                ElapsedTime = $"{timeDif.Minutes}m";
            else
                ElapsedTime = $"{(int) timeDif.TotalHours}h {timeDif.Minutes}m";

            var previousPoints = Trip.Points.Where(p => p.RecordedTimeStamp <= position.RecordedTimeStamp).ToArray();
            var obdPoints = previousPoints.Where(p => p.HasOBDData && p.MassFlowRate > -1).ToArray();

            var totalConsumptionPoints = obdPoints.Length;
            var totalConsumption = obdPoints.Sum(s => s.MassFlowRate);

            if (totalConsumptionPoints > 0)
            {
                var fuelUsedLiters = (totalConsumption/totalConsumptionPoints)*timeDif.TotalHours*0.3047247;
                FuelConsumption = Settings.MetricUnits
                    ? fuelUsedLiters.ToString("N2")
                    : (fuelUsedLiters*.264172).ToString("N2");
            }
            else
            {
                FuelConsumption = "N/A";
            }

            FuelConsumptionUnits = Settings.MetricUnits ? "Liters" : "Gallons";
            DistanceUnits = Settings.MetricDistance ? "Kilometers" : "Miles";

            var currentSpeed = previousPoints.LastOrDefault(s => s.Speed >= 0);

            if (currentSpeed != null)
            {
                Speed = (Settings.Current.MetricDistance ? currentSpeed.Speed : currentSpeed.Speed/1.60934).ToString("f");
            }

            SpeedUnits = Settings.MetricDistance ? "Kmh" : "Mph";

            if (previousPoints.Length > 2)
            {
                double totalDistance = 0;
                var latPrevious = previousPoints[0].Latitude;
                var longPrevious = previousPoints[0].Longitude;
                for (int i = 1; i < previousPoints.Length; i++)
                {
                    var current = previousPoints[i];

                    totalDistance += DistanceUtils.CalculateDistance(current.Latitude, current.Longitude, latPrevious,
                        longPrevious);

                    latPrevious = current.Latitude;
                    longPrevious = current.Longitude;
                }

                Distance = (Settings.Current.MetricDistance ? (totalDistance*1.60934) : totalDistance).ToString("f");
            }
            else
            {
                Distance = "0.0";
            }

            OnPropertyChanged("Stats");
        }
    }
}