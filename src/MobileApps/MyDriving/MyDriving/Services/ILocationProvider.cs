using Plugin.Geolocator.Abstractions;
using System;
using System.Threading.Tasks;

namespace MyDriving.Services
{
    public interface ILocationProvider
    {
        event EventHandler<PositionEventArgs> PositionChanged;
        bool IsListening { get; }
        bool IsGeolocationEnabled { get; }
        bool AllowsBackgroundUpdates { get; set; }
        Task<bool> TryStartAsync();
        Task<bool> TryStopAsync();
        Task<Position> GetPositionAsync(TimeSpan timeout);
    }
}
