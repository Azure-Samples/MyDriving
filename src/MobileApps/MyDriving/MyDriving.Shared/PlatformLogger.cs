// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MyDriving.Utils;
using MyDriving.Utils.Interfaces;

namespace MyDriving.Shared
{
    /// <summary>
    ///     Platform specific logging, mostly for debug write lines or HockeyApp
    /// </summary>
    public class PlatformLogger : Logger
    {
        public override void Identify(string uid, IDictionary<string, string> table = null)
        {
            Debug.WriteLine("Logger: Identify: " + uid);
            base.Identify(uid, table);
        }

        public override void Identify(string uid, string key, string value)
        {
            Debug.WriteLine("Logger: Identify: " + uid + " key: " + key + " value: " + value);



            base.Identify(uid, key, value);
        }

        public override void Track(string trackIdentifier, IDictionary<string, string> table = null)
        {
            Debug.WriteLine("Logger: Track: " + trackIdentifier);
#if __ANDROID__
            HockeyApp.Metrics.MetricsManager.TrackEvent(trackIdentifier);
#elif __IOS__
            HockeyApp.BITHockeyManager.SharedHockeyManager?.MetricsManager?.TrackEvent(trackIdentifier);
#elif WINDOWS_UWP
            Microsoft.HockeyApp.HockeyClient.Current.TrackEvent(trackIdentifier);
#endif
            base.Track(trackIdentifier, table);
        }

        public override void Track(string trackIdentifier, string key, string value)
        {
            Debug.WriteLine("Logger: Track: " + trackIdentifier + " key: " + key + " value: " + value);
#if __ANDROID__
            HockeyApp.Metrics.MetricsManager.TrackEvent(trackIdentifier);
#elif __IOS__
            HockeyApp.BITHockeyManager.SharedHockeyManager?.MetricsManager?.TrackEvent(trackIdentifier);
#elif WINDOWS_UWP
            Microsoft.HockeyApp.HockeyClient.Current.TrackEvent(trackIdentifier);
#endif
            base.Track(trackIdentifier, key, value);
        }

        public override ITrackHandle TrackTime(string identifier, IDictionary<string, string> table = null)
        {
            Debug.WriteLine("Logger: TrackTime: " + identifier);
            Track(identifier);
            return base.TrackTime(identifier, table);
        }

        public override ITrackHandle TrackTime(string identifier, string key, string value)
        {
            Debug.WriteLine("Logger: TrackTime: " + identifier + " key: " + key + " value: " + value);
            Track(identifier);
            return base.TrackTime(identifier, key, value);
        }

        public override void Report(Exception exception)
        {
            Track("Handled: " + exception.ToString());
            Debug.WriteLine("Logger: Report: " + exception);
            base.Report(exception);
        }

        public override Task Save()
        {
            Debug.WriteLine("Logger: Save");
            return base.Save();
        }

        public override Task PurgePendingCrashReports()
        {
            Debug.WriteLine("Logger: PurgePendingCrashReports");
            return base.PurgePendingCrashReports();
        }
    }
}