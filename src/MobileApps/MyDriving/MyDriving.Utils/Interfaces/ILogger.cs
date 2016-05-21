// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

namespace MyDriving.Utils.Interfaces
{
    public enum Severity
    {
        /// <summary>
        ///     Warning Severity
        /// </summary>
        Warning,

        /// <summary>
        ///     Error Severity, you are not expected to call this from client side code unless you have disabled unhandled
        ///     exception handling.
        /// </summary>
        Error,

        /// <summary>
        ///     Critical Severity
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
        void Report(Exception exception);
        Task Save();
        Task PurgePendingCrashReports();
    }

    public interface ITrackHandle
    {
        IDictionary<string, string> Data { get; }
        void Start();
        void Stop();
    }
}