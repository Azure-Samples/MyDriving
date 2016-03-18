// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Android.Content;
using Android.Hardware;
using Java.Lang;
using Exception = System.Exception;
using Math = System.Math;

namespace MyDriving.Droid.Helpers
{
    public interface IAccelerometerListener
    {
        void OnAccelerationChanged(float x, float y, float z);
        void OnShake(float force);
    }

    public class AccelerometerManager
    {
        readonly ShakeSensorEventListener _eventListener;
        readonly SensorManager _sensorManager;
        Sensor _sensor;

        public AccelerometerManager(Context context, IAccelerometerListener listener)
        {
            _eventListener = new ShakeSensorEventListener(listener);
            _sensorManager = (SensorManager) context.GetSystemService(Context.SensorService);
            IsSupported = _sensorManager.GetSensorList(SensorType.Accelerometer).Count > 0;
        }

        public bool IsSupported { get; set; }

        /// <summary>
        ///     Gets if the manager is listening to orientation changes
        /// </summary>
        public bool IsListening { get; private set; }

        public void Configure(int threshold, int interval)
        {
            _eventListener.Threshold = threshold;
            _eventListener.Interval = interval;
        }

        public void StopListening()
        {
            IsListening = false;
            try
            {
                if (_sensorManager != null && _eventListener != null)
                {
                    _sensorManager.UnregisterListener(_eventListener);
                }
            }
            catch (Exception)
            {
            }
        }

        public void StartListening()
        {
            var sensors = _sensorManager.GetSensorList(SensorType.Accelerometer);
            if (sensors.Count > 0)
            {
                _sensor = sensors[0];
                IsListening = _sensorManager.RegisterListener(_eventListener, _sensor, SensorDelay.Game);
            }
        }


        class ShakeSensorEventListener : Object, ISensorEventListener
        {
            readonly IAccelerometerListener _listener;
            long _now, _timeDiff, _lastUpdate, _lastShake;
            float _x, _y, _z, _lastX, _lastY, _lastZ, _force;


            public ShakeSensorEventListener(IAccelerometerListener listener)
            {
                _listener = listener;
                Threshold = 25.0f;
                Interval = 200;
            }

            //Accuracy Configuration
            public float Threshold { get; set; }
            public int Interval { get; set; }

            public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
            {
            }

            public void OnSensorChanged(SensorEvent e)
            {
                try
                {
                    _now = e.Timestamp;
                    _x = e.Values[0];
                    _y = e.Values[1];
                    _z = e.Values[2];

                    if (_lastUpdate == 0)
                    {
                        _lastUpdate = _now;
                        _lastShake = _now;
                        _lastX = _x;
                        _lastY = _y;
                        _lastZ = _z;
                    }
                    else
                    {
                        _timeDiff = _now - _lastUpdate;
                        if (_timeDiff <= 0)
                            return;

                        _force = Math.Abs(_x + _y + _z - _lastX - _lastY - _lastZ);
                        if (Float.Compare(_force, Threshold) > 0)
                        {
                            if (_now - _lastShake >= Interval)
                                _listener.OnShake(_force);

                            _lastShake = _now;
                        }
                        _lastX = _x;
                        _lastY = _y;
                        _lastZ = _z;
                        _lastUpdate = _now;
                    }
                }
                finally
                {
                    _listener.OnAccelerationChanged(_x, _y, _z);
                }
            }
        }
    }
}