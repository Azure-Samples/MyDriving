using System;
using System.Diagnostics;
using System.Collections.Generic;
using MyTrips.Utils.Interfaces;
using System.Collections;
using System.Threading.Tasks;

namespace MyTrips.Utils
{
    public class Logger : ILogger
    {
        public static string HockeyAppiOS => "09f39eb0435c431ebe954f6faf3a1537";
        public static string HockeyAppAndroid => "a8d04f91d07f4e5c91be4034805af61b";
        public static string HockeyAppUWP => "5bff51e242a84d99bddbc6037071656a";
        public static string InsightsKey => "2f8d97df1b77c96616631177848f48954337990a";

        static ILogger instance;
        public static ILogger Instance
        {
            get  { return instance ?? (instance = new Logger()); }
        }
        #region ILogger implementation
        public void Identify(string uid, IDictionary<string, string> table = null)
        {
            Debug.WriteLine("Logger: Identify: " + uid);
            if (!Xamarin.Insights.IsInitialized)
                return;
            Xamarin.Insights.Identify(uid, table);
        }
        public void Identify(string uid, string key, string value)
        {
            Debug.WriteLine("Logger: Identify: " + uid + " key: " + key + " value: " + value);
            if (!Xamarin.Insights.IsInitialized)
                return;
            Xamarin.Insights.Identify(uid, key, value);
        }

        public void Track(string trackIdentifier, IDictionary<string, string> table = null)
        {
            Debug.WriteLine("Logger: Track: " + trackIdentifier);
            if (!Xamarin.Insights.IsInitialized)
                return;
            Xamarin.Insights.Track(trackIdentifier, table);
        }
        public void Track(string trackIdentifier, string key, string value)
        {
            Debug.WriteLine("Logger: Track: " + trackIdentifier + " key: " + key + " value: " + value);
            if (!Xamarin.Insights.IsInitialized)
                return;
            Xamarin.Insights.Track(trackIdentifier, key, value);
        }
        public ITrackHandle TrackTime(string identifier, IDictionary<string, string> table = null)
        {
            Debug.WriteLine("Logger: TrackTime: " + identifier);

            if (!Xamarin.Insights.IsInitialized)
                return null;
            var handle = Xamarin.Insights.TrackTime(identifier, table);
            return new MyTripsTrackHandle(handle);
        }
        public ITrackHandle TrackTime(string identifier, string key, string value)
        {
            Debug.WriteLine("Logger: TrackTime: " + identifier + " key: " + key + " value: " + value);

            if (!Xamarin.Insights.IsInitialized)
                return null;

            var handle = Xamarin.Insights.TrackTime(identifier, key, value);
            return new MyTripsTrackHandle(handle);
        }
        public void Report(Exception exception = null, Severity warningLevel = Severity.Warning)
        {
            Debug.WriteLine("Logger: Report: " + exception);
            if (!Xamarin.Insights.IsInitialized)
                return;

            Xamarin.Insights.Report(exception, GetSeverity(warningLevel));
        }
        public void Report(Exception exception, IDictionary extraData, Severity warningLevel = Severity.Warning)
        {
            Debug.WriteLine("Logger: Report: " + exception);

            if (!Xamarin.Insights.IsInitialized)
                return;
            Xamarin.Insights.Report(exception, extraData, GetSeverity(warningLevel));
        }
        public void Report(Exception exception, string key, string value, Severity warningLevel = Severity.Warning)
        {
            Debug.WriteLine("Logger: Report: " + exception + " key: " + key + " value: " + value);
            if (!Xamarin.Insights.IsInitialized)
                return;
            Xamarin.Insights.Report(exception, key, value, GetSeverity(warningLevel));
        }
        public Task Save()
        {
            Debug.WriteLine("Logger: Save");
            if (!Xamarin.Insights.IsInitialized)
                return null;
            return Xamarin.Insights.Save();
        }
        public Task PurgePendingCrashReports()
        {
            Debug.WriteLine("Logger: PurgePendingCrashReports");
            if (!Xamarin.Insights.IsInitialized)
                return null;
            return Xamarin.Insights.PurgePendingCrashReports();
        }

        public Xamarin.Insights.Severity GetSeverity(Severity severity)
        {
            switch (severity)
            {
                case Severity.Critical:
                return Xamarin.Insights.Severity.Critical;
                case Severity.Error:
                return Xamarin.Insights.Severity.Error;
                default:
                return Xamarin.Insights.Severity.Warning;
            }
        }
        #endregion
    }

    public class MyTripsTrackHandle : ITrackHandle, IDisposable
    {
        readonly Xamarin.ITrackHandle handle;
        public MyTripsTrackHandle(Xamarin.ITrackHandle handle)
        {
            this.handle = handle;
        }

        #region ITrackHandle implementation
        public void Start() => handle?.Start();
        public void Stop() => handle?.Stop();

        public IDictionary<string, string> Data => handle?.Data;

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            handle?.Stop();
            handle?.Dispose();
        }

        #endregion
    }
}

