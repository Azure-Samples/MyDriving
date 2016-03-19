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
        readonly ShakeSensorEventListener eventListener;
        readonly SensorManager sensorManager;
        Sensor sensor;

        public AccelerometerManager(Context context, IAccelerometerListener listener)
        {
            eventListener = new ShakeSensorEventListener(listener);
            sensorManager = (SensorManager) context.GetSystemService(Context.SensorService);
            IsSupported = sensorManager.GetSensorList(SensorType.Accelerometer).Count > 0;
        }

        public bool IsSupported { get; set; }

        /// <summary>
        ///     Gets if the manager is listening to orientation changes
        /// </summary>
        public bool IsListening { get; private set; }

        public void Configure(int threshold, int interval)
        {
            eventListener.Threshold = threshold;
            eventListener.Interval = interval;
        }

        public void StopListening()
        {
            IsListening = false;
            try
            {
                if (sensorManager != null && eventListener != null)
                {
                    sensorManager.UnregisterListener(eventListener);
                }
            }
            catch (Exception)
            {
            }
        }

        public void StartListening()
        {
            var sensors = sensorManager.GetSensorList(SensorType.Accelerometer);
            if (sensors.Count > 0)
            {
                sensor = sensors[0];
                IsListening = sensorManager.RegisterListener(eventListener, sensor, SensorDelay.Game);
            }
        }


        class ShakeSensorEventListener : Object, ISensorEventListener
        {
            readonly IAccelerometerListener listener;
            long now, timeDiff, lastUpdate, lastShake;
            float x, y, z, lastX, lastY, lastZ, force;


            public ShakeSensorEventListener(IAccelerometerListener listener)
            {
                this.listener = listener;
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
                    now = e.Timestamp;
                    x = e.Values[0];
                    y = e.Values[1];
                    z = e.Values[2];

                    if (lastUpdate == 0)
                    {
                        lastUpdate = now;
                        lastShake = now;
                        lastX = x;
                        lastY = y;
                        lastZ = z;
                    }
                    else
                    {
                        timeDiff = now - lastUpdate;
                        if (timeDiff <= 0)
                            return;

                        force = Math.Abs(x + y + z - lastX - lastY - lastZ);
                        if (Float.Compare(force, Threshold) > 0)
                        {
                            if (now - lastShake >= Interval)
                                listener.OnShake(force);

                            lastShake = now;
                        }
                        lastX = x;
                        lastY = y;
                        lastZ = z;
                        lastUpdate = now;
                    }
                }
                finally
                {
                    listener.OnAccelerationChanged(x, y, z);
                }
            }
        }
    }
}