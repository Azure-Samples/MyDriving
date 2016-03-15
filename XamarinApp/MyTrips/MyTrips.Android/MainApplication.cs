using System;

using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;
using MyTrips.Utils;
using MyTrips.Interfaces;
using MyTrips.Droid.Helpers;
using Acr.UserDialogs;
using MyTrips.Shared;
using MyTrips.DataStore.Abstractions;

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
            ServiceLocator.Instance.Add<MyTrips.Utils.Interfaces.ILogger, MyTrips.Shared.PlatformLogger>();
            ServiceLocator.Instance.Add<IHubIOT, IOTHub>();
            ServiceLocator.Instance.Add<IOBDDevice, OBDDevice>();

            ServiceLocator.Instance.Add<IHubIOTStore, MyTrips.DataStore.Mock.Stores.IOTHubStore>();
            //ServiceLocator.Instance.Add<IHubIOTStore, MyTrips.DataStore.Azure.Stores.IOTHubStore>();

            Xamarin.Insights.Initialize(Logger.InsightsKey, this);
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            UserDialogs.Init(() => CrossCurrentActivity.Current.Activity);
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