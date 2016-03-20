// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using MyDriving.Utils.Interfaces;
using System.Collections;
using System.Threading.Tasks;

namespace MyDriving.Utils
{
    public class Logger : ILogger
    {
        static ILogger _instance;
        public static string HockeyAppiOS => "09f39eb0435c431ebe954f6faf3a1537";
        public static string HockeyAppAndroid => "a8d04f91d07f4e5c91be4034805af61b";
        public static string HockeyAppUWP => "5bff51e242a84d99bddbc6037071656a";
        public static string InsightsKey => "2f8d97df1b77c96616631177848f48954337990a";

        public static ILogger Instance => _instance ?? (_instance = ServiceLocator.Instance.Resolve<ILogger>());

        #region ILogger implementation

        public virtual void WriteLine(string line)
        {
        }

        public virtual void Identify(string uid, IDictionary<string, string> table = null)
        {
            if (!Xamarin.Insights.IsInitialized)
                return;
            Xamarin.Insights.Identify(uid, table);
        }

        public virtual void Identify(string uid, string key, string value)
        {
            if (!Xamarin.Insights.IsInitialized)
                return;
            Xamarin.Insights.Identify(uid, key, value);
        }

        public virtual void Track(string trackIdentifier, IDictionary<string, string> table = null)
        {
            if (!Xamarin.Insights.IsInitialized)
                return;
            Xamarin.Insights.Track(trackIdentifier, table);
        }

        public virtual void Track(string trackIdentifier, string key, string value)
        {
            if (!Xamarin.Insights.IsInitialized)
                return;
            Xamarin.Insights.Track(trackIdentifier, key, value);
        }

        public virtual ITrackHandle TrackTime(string identifier, IDictionary<string, string> table = null)
        {
            if (!Xamarin.Insights.IsInitialized)
                return null;
            var handle = Xamarin.Insights.TrackTime(identifier, table);
            return new MyDrivingTrackHandle(handle);
        }

        public virtual ITrackHandle TrackTime(string identifier, string key, string value)
        {
            if (!Xamarin.Insights.IsInitialized)
                return null;

            var handle = Xamarin.Insights.TrackTime(identifier, key, value);
            return new MyDrivingTrackHandle(handle);
        }

        public virtual void Report(Exception exception = null, Severity warningLevel = Severity.Warning)
        {
            if (!Xamarin.Insights.IsInitialized)
                return;

            Xamarin.Insights.Report(exception, GetSeverity(warningLevel));
        }

        public virtual void Report(Exception exception, IDictionary extraData, Severity warningLevel = Severity.Warning)
        {
            if (!Xamarin.Insights.IsInitialized)
                return;
            Xamarin.Insights.Report(exception, extraData, GetSeverity(warningLevel));
        }

        public virtual void Report(Exception exception, string key, string value,
            Severity warningLevel = Severity.Warning)
        {
            if (!Xamarin.Insights.IsInitialized)
                return;
            Xamarin.Insights.Report(exception, key, value, GetSeverity(warningLevel));
        }

        public virtual Task Save()
        {
            if (!Xamarin.Insights.IsInitialized)
                return null;
            return Xamarin.Insights.Save();
        }

        public virtual Task PurgePendingCrashReports()
        {
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

    public class MyDrivingTrackHandle : ITrackHandle, IDisposable
    {
        readonly Xamarin.ITrackHandle handle;

        public MyDrivingTrackHandle(Xamarin.ITrackHandle handle)
        {
            this.handle = handle;
        }

        #region IDisposable implementation

        public void Dispose()
        {
            handle?.Stop();
            handle?.Dispose();
        }

        #endregion

        #region ITrackHandle implementation

        public void Start() => handle?.Start();
        public void Stop() => handle?.Stop();

        public IDictionary<string, string> Data => handle?.Data;

        #endregion
    }
}