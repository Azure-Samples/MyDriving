// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Android.App;
using Android.OS;
using Android.Content;
using MyDriving.ViewModel;
using System;
using Android.Support.V4.App;

namespace MyDriving.Droid.Services
{
    public class GeolocationServiceBinder : Binder
    {
        public GeolocationServiceBinder(GeolocationService service)
        {
            Service = service;
        }

        public GeolocationService Service { get; }

        public bool IsBound { get; set; }
    }

    [Service]
    public class GeolocationService : Service
    {
        IBinder binder;

        public CurrentTripViewModel ViewModel { get; private set; }

        public override IBinder OnBind(Intent intent)
        {
            binder = new GeolocationServiceBinder(this);
            return binder;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var builder = new NotificationCompat.Builder(this);

            var newIntent = new Intent(this, typeof (MainActivity));
            newIntent.PutExtra("tracking", true);
            newIntent.AddFlags(ActivityFlags.ClearTop);
            newIntent.AddFlags(ActivityFlags.SingleTop);

            var pendingIntent = PendingIntent.GetActivity(this, 0, newIntent, 0);
            var notification = builder.SetContentIntent(pendingIntent)
                .SetSmallIcon(Resource.Drawable.ic_notification)
                .SetAutoCancel(false)
                .SetTicker("MyDriving is recording.")
                .SetContentTitle("MyDriving")
                .SetContentText("MyDriving is recording your current trip.")
                .Build();


            StartForeground((int) NotificationFlags.ForegroundService, notification);

            ViewModel = new CurrentTripViewModel();
            return StartCommandResult.Sticky;
        }

        public void StartLocationUpdates()
        {
            ViewModel.StartTrackingTripCommand.Execute(null);
        }

        public void StopLocationUpdates()
        {
            ViewModel.StopTrackingTripCommand.Execute(null);
        }
    }

    public class GeolocationServiceConnection : Java.Lang.Object, IServiceConnection
    {
        public GeolocationServiceConnection(GeolocationServiceBinder binder)
        {
            if (binder != null)
            {
                Binder = binder;
            }
        }

        public GeolocationServiceBinder Binder { get; set; }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            var serviceBinder = service as GeolocationServiceBinder;

            if (serviceBinder == null)
                return;


            Binder = serviceBinder;
            Binder.IsBound = true;

            // raise the service bound event
            ServiceConnected?.Invoke(this, new ServiceConnectedEventArgs {Binder = service});

            // begin updating the location in the Service
            serviceBinder.Service.StartLocationUpdates();
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Binder.IsBound = false;
        }

        public event EventHandler<ServiceConnectedEventArgs> ServiceConnected;
    }

    public class ServiceConnectedEventArgs : EventArgs
    {
        public IBinder Binder { get; set; }
    }
}