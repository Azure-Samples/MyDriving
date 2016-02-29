using System;

using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;
using MyTrips.Utils;
using MyTrips.Interfaces;
using MyTrips.Droid.Helpers;

namespace MyTrips.Droid
{
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
            ViewModel.ViewModelBase.Init();
            ServiceLocator.Instance.Add<IAuthentication, Authentication>();
            Xamarin.Insights.Initialize(Logger.InsightsKey, this);
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState) => CrossCurrentActivity.Current.Activity = activity;

        public void OnActivityDestroyed(Activity activity) { }

        public void OnActivityPaused(Activity activity) { }

        public void OnActivityResumed(Activity activity) => CrossCurrentActivity.Current.Activity = activity;


        public void OnActivitySaveInstanceState(Activity activity, Bundle outState) {}

        public void OnActivityStarted(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
            HockeyApp.Tracking.StartUsage(activity);
        }

        public void OnActivityStopped(Activity activity) => HockeyApp.Tracking.StopUsage(activity);

    }
}