// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using MyDriving.Utils;

namespace MyDriving.Droid.Activities
{
    [Activity(Label = "MyDriving", Theme = "@style/SplashTheme", MainLauncher = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Intent newIntent;
            if (Settings.Current.IsLoggedIn)
            {
                newIntent = new Intent(this, typeof(MainActivity));

                //When the first screen of the app is launched after user has logged in, initialize the processor that manages connection to OBD Device and to the IOT Hub
                MyDriving.Services.OBDDataProcessor.GetProcessor().Initialize(ViewModel.ViewModelBase.StoreManager);
            }

            else if (Settings.Current.FirstRun)
            {
#if XTC
                newIntent = new Intent(this, typeof(LoginActivity));

#else
                newIntent = new Intent(this, typeof(GettingStartedActivity));

#endif

#if !DEBUG
                Settings.Current.FirstRun = false;
#endif
            }
            else
                newIntent = new Intent(this, typeof(LoginActivity));


            newIntent.AddFlags(ActivityFlags.ClearTop);
            newIntent.AddFlags(ActivityFlags.SingleTop);
            StartActivity(newIntent);
            Finish();
        }
    }
}