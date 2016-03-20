// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Android.OS;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Views;
using Android.Widget;
using Android.Content;
using MyDriving.Droid.Activities;

namespace MyDriving.Droid.Fragments
{
    public class FragmentGettingStarted1 : Fragment
    {
        public static FragmentGettingStarted1 NewInstance() => new FragmentGettingStarted1 {Arguments = new Bundle()};

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.fragment_started_1, null);
        }
    }

    public class FragmentGettingStarted2 : Fragment
    {
        public static FragmentGettingStarted2 NewInstance() => new FragmentGettingStarted2 {Arguments = new Bundle()};

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.fragment_started_2, null);
        }
    }

    public class FragmentGettingStarted3 : Fragment
    {
        public static FragmentGettingStarted3 NewInstance() => new FragmentGettingStarted3 {Arguments = new Bundle()};

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.fragment_started_3, null);
        }
    }

    public class FragmentGettingStarted4 : Fragment
    {
        public static FragmentGettingStarted4 NewInstance() => new FragmentGettingStarted4 {Arguments = new Bundle()};

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.fragment_started_4, null);
        }
    }

    public class FragmentGettingStarted5 : Fragment
    {
        public static FragmentGettingStarted5 NewInstance() => new FragmentGettingStarted5 {Arguments = new Bundle()};

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_started_5, null);


            view.FindViewById<Button>(Resource.Id.button_close).Click += (sender, args) =>
            {
                var intent = new Intent(Activity, typeof (LoginActivity));
                intent.AddFlags(ActivityFlags.ClearTop);
                Activity.StartActivity(intent);
                Activity.Finish();
            };

            return view;
        }
    }
}