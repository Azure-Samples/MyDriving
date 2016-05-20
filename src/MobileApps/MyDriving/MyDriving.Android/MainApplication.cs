// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;
using MyDriving.Utils;
using MyDriving.Utils.Interfaces;
using MyDriving.Interfaces;
using MyDriving.Droid.Helpers;
using Acr.UserDialogs;
using MyDriving.Shared;

namespace MyDriving.Droid
{
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
            : base(handle, transer)
        {
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
            => CrossCurrentActivity.Current.Activity = activity;

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity) => CrossCurrentActivity.Current.Activity = activity;

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

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

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
            ViewModel.ViewModelBase.Init();
            ServiceLocator.Instance.Add<IAuthentication, Authentication>();
            ServiceLocator.Instance.Add<Utils.Interfaces.ILogger, PlatformLogger>();
            ServiceLocator.Instance.Add<IOBDDevice, OBDDevice>();

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            UserDialogs.Init(() => CrossCurrentActivity.Current.Activity);
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }
    }
}