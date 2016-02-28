using Android.App;
using Android.OS;
using Android.Content;
using MyTrips.ViewModel;
using System;
using Android.Support.V4.App;

namespace MyTrips.Droid.Services
{

    public class GeolocationServiceBinder : Binder
    {
        public GeolocationService Service { get; }

        public bool IsBound { get; set; }

        public GeolocationServiceBinder(GeolocationService service)
        {
            Service = service;
        }
    }

    [Service]
    public class GeolocationService : Service
    {
        IBinder binder;

        public override IBinder OnBind(Intent intent)
        {
            binder = new GeolocationServiceBinder(this);
            return binder;
        }

        public CurrentTripViewModel ViewModel { get; private set; }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var builder = new NotificationCompat.Builder(this);

            var newIntent = new Intent(this, typeof(MainActivity));
            newIntent.PutExtra("tracking", true);
            newIntent.AddFlags(ActivityFlags.ClearTop);
            newIntent.AddFlags(ActivityFlags.SingleTop);

            var pendingIntent = PendingIntent.GetActivity (this, 0, newIntent, 0);
            var notification = builder.SetContentIntent(pendingIntent)
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetAutoCancel(false)
                .SetTicker("MyTrips in foreground")
                .SetContentTitle("MyTrips")
                .SetContentText("MyTrips is running in the foreground")
                .Build();
            

            StartForeground ((int)NotificationFlags.ForegroundService, notification);

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
        public event EventHandler<ServiceConnectedEventArgs> ServiceConnected;

        public GeolocationServiceBinder Binder { get; set; }

        public GeolocationServiceConnection(GeolocationServiceBinder binder)
        {
            if (binder != null)
            {
                Binder = binder;
            }
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            var serviceBinder = service as  GeolocationServiceBinder;

            if (serviceBinder == null)
                return;
            

            Binder = serviceBinder;
            Binder.IsBound = true;

            // raise the service bound event
            ServiceConnected?.Invoke(this, new ServiceConnectedEventArgs { Binder = service });

            // begin updating the location in the Service
            serviceBinder.Service.StartLocationUpdates();

        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Binder.IsBound = false;
        }
    }

    public class ServiceConnectedEventArgs : EventArgs
    {
        public IBinder Binder { get; set; }
    }
}

