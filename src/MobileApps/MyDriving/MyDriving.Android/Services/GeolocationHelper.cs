// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Threading.Tasks;
using Android.Util;
using Android.Content;

namespace MyDriving.Droid.Services
{
    public class GeolocationHelper
    {
        protected static GeolocationServiceConnection LocationServiceConnection;

        private static bool _isRunning;

        // declarations
        protected readonly string LogTag = "App";

        // properties

        public static GeolocationHelper Current { get; }

        public bool IsRunning => _isRunning;

        public GeolocationService LocationService
        {
            get
            {
                if (LocationServiceConnection.Binder == null)
                    throw new Exception("Service not bound yet");
                // note that we use the ServiceConnection to get the Binder, and the Binder to get the Service here
                return LocationServiceConnection.Binder.Service;
            }
        }

        // events
        public event EventHandler<ServiceConnectedEventArgs> LocationServiceConnected = delegate { };

        #region Application context

        static GeolocationHelper()
        {
            Current = new GeolocationHelper();
        }

        protected GeolocationHelper()
        {
            // create a new service connection so we can get a binder to the service
            LocationServiceConnection = new GeolocationServiceConnection(null);

            // this event will fire when the Service connectin in the OnServiceConnected call 
            LocationServiceConnection.ServiceConnected += (sender, e) =>
            {
                Log.Debug(LogTag, "Service Connected");
                // we will use this event to notify MainActivity when to start updating the UI
                LocationServiceConnected(this, e);
            };
        }

        public static Task StartLocationService()
        {
            if (_isRunning)
                return Task.FromResult(true);
            _isRunning = true;
            // Starting a service like this is blocking, so we want to do it on a background thread
            return Task.Run(() =>
            {
                // Start our main service
                Log.Debug("App", "Calling StartService");
                Android.App.Application.Context.StartService(new Intent(Android.App.Application.Context,
                    typeof (GeolocationService)));

                // bind our service (Android goes and finds the running service by type, and puts a reference
                // on the binder to that service)
                // The Intent tells the OS where to find our Service (the Context) and the Type of Service
                // we're looking for (LocationService)
                var locationServiceIntent = new Intent(Android.App.Application.Context, typeof (GeolocationService));
                Log.Debug("App", "Calling service binding");

                // Finally, we can bind to the Service using our Intent and the ServiceConnection we
                // created in a previous step.
                Android.App.Application.Context.BindService(locationServiceIntent, LocationServiceConnection,
                    Bind.AutoCreate);
            });
        }

        public static void StopLocationService()
        {
            try
            {
                if (!_isRunning)
                    return;
                _isRunning = false;
                // Check for nulls in case StartLocationService task has not yet completed.
                Log.Debug("App", "StopLocationService");

                // Unbind from the LocationService; otherwise, StopSelf (below) will not work:
                if (LocationServiceConnection != null)
                {
                    Log.Debug("App", "Unbinding from LocationService");
                    Android.App.Application.Context.UnbindService(LocationServiceConnection);
                }

                // Stop the LocationService:
                if (Current.LocationService != null)
                {
                    Log.Debug("App", "Stopping the LocationService");
                    Current.LocationService.StopSelf();
                }
            }
            catch
            {
            }
        }

        #endregion
    }
}