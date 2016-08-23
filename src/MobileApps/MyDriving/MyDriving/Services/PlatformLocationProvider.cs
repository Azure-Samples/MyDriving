using Plugin.DeviceInfo;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Threading.Tasks;

namespace MyDriving.Services
{
    public class PlatformLocationProvider : ILocationProvider
    {
        public static readonly PlatformLocationProvider Instance = new PlatformLocationProvider();

        private readonly IGeolocator geolocator;

        public event EventHandler<PositionEventArgs> PositionChanged;

        private PlatformLocationProvider()
        {
            geolocator = CrossGeolocator.Current;
        }

        public bool IsListening { get { return geolocator.IsListening; } }
        public bool IsGeolocationEnabled { get { return geolocator.IsGeolocationEnabled; } }
        public bool AllowsBackgroundUpdates { get { return geolocator.AllowsBackgroundUpdates; } set { geolocator.AllowsBackgroundUpdates = value; }}

        public async Task<bool> TryStartAsync()
        {
            if (geolocator.IsGeolocationAvailable 
                && (CrossDeviceInfo.Current.Platform == Plugin.DeviceInfo.Abstractions.Platform.iOS || geolocator.IsGeolocationEnabled))
            {
                geolocator.AllowsBackgroundUpdates = true;
                geolocator.DesiredAccuracy = 25;

                geolocator.PositionChanged += Geolocator_PositionChanged;

                //every 3 second, 5 meters
                return await geolocator.StartListeningAsync(3000, 5);
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> TryStopAsync()
        {
            return await geolocator.StopListeningAsync();
        }

        public async Task<Position> GetPositionAsync(TimeSpan timeout)
        {
            return await geolocator.GetPositionAsync((int)timeout.TotalMilliseconds);
        }

        private void Geolocator_PositionChanged(object sender, PositionEventArgs e)
        {
            PositionChanged?.Invoke(this, e);
        }
    }
}
