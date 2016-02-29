using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

namespace MyTrips.Utils.Interfaces
{
    public enum Severity
    {
        /// <summary>
        /// Warning Severity
        /// </summary>
        Warning,
        /// <summary>
        /// Error Severity, you are not expected to call this from client side code unless you have disabled unhandled exception handling.
        /// </summary>
        Error,
        /// <summary>
        /// Critical Severity
        /// </summary>
        Critical
    }

    public interface ILogger
    {
        void Identify(string uid, IDictionary<string, string> table = null);
        void Identify(string uid, string key, string value);
        void Track(string trackIdentifier, IDictionary<string, string> table = null);
        void Track(string trackIdentifier, string key, string value);
        ITrackHandle TrackTime(string identifier, IDictionary<string, string> table = null);
        ITrackHandle TrackTime(string identifier, string key, string value);
        void Report(Exception exception = null, Severity warningLevel = Severity.Warning);
        void Report(Exception exception, IDictionary extraData, Severity warningLevel = Severity.Warning);
        void Report(Exception exception, string key, string value, Severity warningLevel = Severity.Warning);
        Task Save();
        Task PurgePendingCrashReports();
    }

    public interface ITrackHandle
    {
        void Start();
        void Stop();
        IDictionary<string, string> Data { get; }
    }
}

