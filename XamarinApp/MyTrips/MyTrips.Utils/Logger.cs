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

        static ILogger instance;
        public static ILogger Instance
        {
            get  { return instance ?? (instance = new Logger()); }
        }
        #region ILogger implementation
        public virtual void Identify(string uid, IDictionary<string, string> table = null)
        {
            Debug.WriteLine("Evolve Logger: Identify: " + uid);
        }
        public virtual  void Identify(string uid, string key, string value)
        {
            Debug.WriteLine("Evolve Logger: Identify: " + uid + " key: " + key + " value: " + value);
        }
       
        public virtual  void Track(string trackIdentifier, IDictionary<string, string> table = null)
        {
            Debug.WriteLine("Evolve Logger: Track: " + trackIdentifier);
        }
        public virtual  void Track(string trackIdentifier, string key, string value)
        {
            Debug.WriteLine("Evolve Logger: Track: " + trackIdentifier + " key: " + key + " value: " + value);
     
        }
       
        public virtual  void Report(Exception exception = null, Severity warningLevel = Severity.Warning)
        {
            Debug.WriteLine("Evolve Logger: Report: " + exception);
        }
        public virtual  void Report(Exception exception, IDictionary extraData, Severity warningLevel = Severity.Warning)
        {
            Debug.WriteLine("Evolve Logger: Report: " + exception);
        }
        public virtual  void Report(Exception exception, string key, string value, Severity warningLevel = Severity.Warning)
        {
            Debug.WriteLine("Evolve Logger: Report: " + exception + " key: " + key + " value: " + value);
        }
        public virtual  Task Save()
        {
            Debug.WriteLine("Evolve Logger: Save");
            return Task.FromResult(true);
        }
        public virtual  Task PurgePendingCrashReports()
        {
            Debug.WriteLine("Evolve Logger: PurgePendingCrashReports");
            return Task.FromResult(true);
        }
            
        #endregion
    }
}

