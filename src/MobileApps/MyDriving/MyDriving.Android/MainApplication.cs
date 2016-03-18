using System;

using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;
using MyDriving.Utils;
using MyDriving.Interfaces;
using MyDriving.Droid.Helpers;
using Acr.UserDialogs;
using MyDriving.Shared;
using MyDriving.DataStore.Abstractions;

namespace MyDriving.Droid
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
            ServiceLocator.Instance.Add<MyDriving.Utils.Interfaces.ILogger, MyDriving.Shared.PlatformLogger>();
            ServiceLocator.Instance.Add<IHubIOT, IOTHub>();
            ServiceLocator.Instance.Add<IOBDDevice, OBDDevice>();

            //When the first screen of the app is launched after user has logged in, initialize the processor that manages connection to OBD Device and to the IOT Hub
            MyDriving.Services.OBDDataProcessor.GetProcessor().Initialize(ViewModel.ViewModelBase.StoreManager);

            #if !XTC
            Xamarin.Insights.Initialize(Logger.InsightsKey, this);
            #endif

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
            #if !XTC
            HockeyApp.Tracking.StartUsage(activity);
            #endif
        }

        public void OnActivityStopped(Activity activity)
        {
            #if !XTC
            HockeyApp.Tracking.StopUsage(activity);
            #endif
        }

    }
}