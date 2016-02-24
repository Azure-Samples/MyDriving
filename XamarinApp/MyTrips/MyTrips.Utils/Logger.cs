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
        static ILogger instance;
        public static ILogger Instance
        {
            get  { return instance ?? (instance = new Logger()); }
        }
        #region ILogger implementation
        public void Identify(string uid, IDictionary<string, string> table = null)
        {
            Debug.WriteLine("Evolve Logger: Identify: " + uid);
        }
        public void Identify(string uid, string key, string value)
        {
            Debug.WriteLine("Evolve Logger: Identify: " + uid + " key: " + key + " value: " + value);
        }
       
        public void Track(string trackIdentifier, IDictionary<string, string> table = null)
        {
            Debug.WriteLine("Evolve Logger: Track: " + trackIdentifier);
        }
        public void Track(string trackIdentifier, string key, string value)
        {
            Debug.WriteLine("Evolve Logger: Track: " + trackIdentifier + " key: " + key + " value: " + value);
     
        }
       
        public void Report(Exception exception = null, Severity warningLevel = Severity.Warning)
        {
            Debug.WriteLine("Evolve Logger: Report: " + exception);
        }
        public void Report(Exception exception, IDictionary extraData, Severity warningLevel = Severity.Warning)
        {
            Debug.WriteLine("Evolve Logger: Report: " + exception);
        }
        public void Report(Exception exception, string key, string value, Severity warningLevel = Severity.Warning)
        {
            Debug.WriteLine("Evolve Logger: Report: " + exception + " key: " + key + " value: " + value);
        }
        public Task Save()
        {
            Debug.WriteLine("Evolve Logger: Save");
            return Task.FromResult(true);
        }
        public Task PurgePendingCrashReports()
        {
            Debug.WriteLine("Evolve Logger: PurgePendingCrashReports");
            return Task.FromResult(true);
        }
            
        #endregion
    }
}

