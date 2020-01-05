using Plugin.Geolocator.Abstractions;
using System;
using System.Threading.Tasks;

namespace MyDriving.Services
{
    public class FakeLocationProvider : ILocationProvider
    {
        bool running;
        Position lastPosition;

        public event EventHandler<PositionEventArgs> PositionChanged;

        public FakeLocationProvider(Position initalPosition)
        {
            lastPosition = initalPosition;
        }

        public bool IsListening { get { return running; } }
        public bool IsGeolocationEnabled { get { return true; } }
        public bool AllowsBackgroundUpdates { get; set; } // TODO unset running when app goes into background if this is false

        public async Task<bool> TryStartAsync()
        {
            if (running)
                throw new InvalidOperationException("already started");

            running = true;

            while (true)
            {
                var pos = new Position(lastPosition);
                pos.Longitude += 0.00085720162;
                PositionChanged?.Invoke(this, new PositionEventArgs(lastPosition));
                lastPosition = pos;

                await Task.Delay(3000);

                if (!running)
                {
                    return true;
                };
            }
        }

        public async Task<bool> TryStopAsync()
        {
            running = false;
            await Task.Delay(1000); // fake work
            return true;
        }
        
        public async Task<Position> GetPositionAsync(TimeSpan timeout)
        {
            await Task.Delay(100); // fake work
            return new Position(lastPosition);
        }
    }
}
