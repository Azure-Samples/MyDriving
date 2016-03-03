using System;
using System.Diagnostics;
using System.Collections.Generic;
using MyTrips.Utils.Interfaces;
using System.Collections;
using System.Threading.Tasks;
using MyTrips.Utils;

namespace MyTrips.Shared
{
    /// <summary>
    /// Platform specific logging, mostly for debug write lines or HockeyApp
    /// </summary>
    public class PlatformLogger : Logger
    {
        public override void Identify(string uid, IDictionary<string, string> table = null)
        {
            Debug.WriteLine("Logger: Identify: " + uid);
            base.Identify(uid, table);
        }

        public override void WriteLine(string line)
        {
            Debug.WriteLine(line);
            base.WriteLine(line);
        }
        public override void Identify(string uid, string key, string value)
        {
            Debug.WriteLine("Logger: Identify: " + uid + " key: " + key + " value: " + value);
            base.Identify(uid, key, value);
        }

        public override  void Track(string trackIdentifier, IDictionary<string, string> table = null)
        {
            Debug.WriteLine("Logger: Track: " + trackIdentifier);
            base.Track(trackIdentifier, table);
        }
        public override void Track(string trackIdentifier, string key, string value)
        {
            Debug.WriteLine("Logger: Track: " + trackIdentifier + " key: " + key + " value: " + value);
            base.Track(trackIdentifier, key, value);
        }
        public override ITrackHandle TrackTime(string identifier, IDictionary<string, string> table = null)
        {
            Debug.WriteLine("Logger: TrackTime: " + identifier);

            return base.TrackTime(identifier, table);
        }
        public override ITrackHandle TrackTime(string identifier, string key, string value)
        {
            Debug.WriteLine("Logger: TrackTime: " + identifier + " key: " + key + " value: " + value);

            return base.TrackTime(identifier, key, value);
        }
        public override void Report(Exception exception = null, Severity warningLevel = Severity.Warning)
        {
            Debug.WriteLine("Logger: Report: " + exception);
            base.Report(exception, warningLevel);
        }
        public override void Report(Exception exception, IDictionary extraData, Severity warningLevel = Severity.Warning)
        {
            Debug.WriteLine("Logger: Report: " + exception);

            base.Report(exception, extraData, warningLevel);
        }
        public override void Report(Exception exception, string key, string value, Severity warningLevel = Severity.Warning)
        {
            Debug.WriteLine("Logger: Report: " + exception + " key: " + key + " value: " + value);
            base.Report(exception, key, value, warningLevel);
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

