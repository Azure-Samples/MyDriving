using System;
namespace MyTrips.Droid.Helpers
{
    public interface IAccelerometerListener
    {
        void OnAccelerationChanged(float x, float y, float z);
        void OnShake(float force);
    }
}

