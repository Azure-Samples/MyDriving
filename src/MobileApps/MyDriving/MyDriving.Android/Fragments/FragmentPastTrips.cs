// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using Android.Widget;
using MyDriving.ViewModel;
using System;
using MyDriving.Droid.Activities;
using Android.Content;


namespace MyDriving.Droid.Fragments
{
    public class FragmentPastTrips : Fragment
    {
        TripAdapter adapter;
        LinearLayoutManager layoutManager;

        RecyclerView recyclerView;
        SwipeRefreshLayout refresher;
        PastTripsViewModel viewModel;

        public static FragmentPastTrips NewInstance() => new FragmentPastTrips {Arguments = new Bundle()};

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_past_trips, null);

            viewModel = new PastTripsViewModel();

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            refresher = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);

            refresher.Refresh += (sender, e) => viewModel.LoadPastTripsCommand.Execute(null);


            adapter = new TripAdapter(Activity, viewModel);
            adapter.ItemClick += OnItemClick;
            adapter.ItemLongClick += OnItemLongClick;
            layoutManager = new LinearLayoutManager(Activity) {Orientation = LinearLayoutManager.Vertical};
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.SetAdapter(adapter);
            recyclerView.ClearOnScrollListeners();
            recyclerView.AddOnScrollListener(new TripsOnScrollListenerListener(viewModel, layoutManager));

            return view;
        }

        public override async void OnStart()
        {
            base.OnStart();
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            if (viewModel.Trips.Count == 0)
                await viewModel.ExecuteLoadPastTripsCommandAsync();
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(viewModel.IsBusy):
                    refresher.Refreshing = viewModel.IsBusy;
                    break;
            }
        }

        public override void OnStop()
        {
            base.OnStop();
            viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        void OnItemClick(object sender, TripClickEventArgs args)
        {
            var trip = viewModel.Trips[args.Position];
            var intent = new Intent(Activity, typeof (PastTripDetailsActivity));
            intent.PutExtra("Id", trip.Id);
            intent.PutExtra("Rating", trip.Rating);

            PastTripDetailsActivity.Trip = trip;
            Activity.StartActivity(intent);
        }

        async void OnItemLongClick(object sender, TripClickEventArgs args)
        {
            var trip = viewModel.Trips[args.Position];
            await viewModel.ExecuteDeleteTripCommand(trip);
        }

        class TripsOnScrollListenerListener : RecyclerView.OnScrollListener
        {
            readonly LinearLayoutManager layoutManager;
            readonly PastTripsViewModel viewModel;

            public TripsOnScrollListenerListener(PastTripsViewModel viewModel, LinearLayoutManager layoutManager)
            {
                this.layoutManager = layoutManager;
                this.viewModel = viewModel;
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                base.OnScrolled(recyclerView, dx, dy);
                if (viewModel.IsBusy || viewModel.Trips.Count == 0 || !viewModel.CanLoadMore)
                    return;

                var lastVisiblePosition = layoutManager.FindLastCompletelyVisibleItemPosition();
                if (lastVisiblePosition == RecyclerView.NoPosition)
                    return;

                //if we are at the bottom and can load more.
                if (lastVisiblePosition == viewModel.Trips.Count - 1)
                    viewModel.LoadMorePastTripCommand.Execute(null);
            }
        }
    }


    public class TripViewHolder : RecyclerView.ViewHolder
    {
        public TripViewHolder(View itemView, Action<TripClickEventArgs> listener,
            Action<TripClickEventArgs> listenerLong) : base(itemView)
        {
            itemView.LongClickable = true;
            Title = itemView.FindViewById<TextView>(Resource.Id.text_title);
            Distance = itemView.FindViewById<TextView>(Resource.Id.text_distance);
            Date = itemView.FindViewById<TextView>(Resource.Id.text_date);
            Photo = itemView.FindViewById<ImageView>(Resource.Id.photo);
            itemView.Click +=
                (sender, e) => listener(new TripClickEventArgs {View = sender as View, Position = AdapterPosition});
            itemView.LongClick +=
                (sender, e) => listenerLong(new TripClickEventArgs {View = sender as View, Position = AdapterPosition});
        }

        public TextView Title { get; set; }
        public TextView Date { get; set; }
        public TextView Distance { get; set; }
        public ImageView Photo { get; set; }
    }

    public class TripClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }

    public class TripAdapter : RecyclerView.Adapter
    {
        readonly Android.App.Activity activity;
        readonly PastTripsViewModel viewModel;

        public TripAdapter(Android.App.Activity activity, PastTripsViewModel viewModel)
        {
            this.activity = activity;
            this.viewModel = viewModel;

            this.viewModel.Trips.CollectionChanged +=
                (sender, e) => { this.activity.RunOnUiThread(NotifyDataSetChanged); };
        }

        public override int ItemCount => viewModel.Trips.Count;

        public event EventHandler<TripClickEventArgs> ItemClick;
        public event EventHandler<TripClickEventArgs> ItemLongClick;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
            new TripViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_trip, parent, false),
                OnClick, OnClickLong);

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as TripViewHolder;
            if (vh == null)
                return;

            var trip = viewModel.Trips[position];
            vh.Title.Text = trip.Name;
            vh.Distance.Text = trip.TotalDistance;
            vh.Date.Text = trip.TimeAgo;
            vh.Photo.Visibility = (trip?.Photos?.Count).GetValueOrDefault() > 0 ||
                                  !string.IsNullOrWhiteSpace(trip.MainPhotoUrl)
                ? ViewStates.Visible
                : ViewStates.Gone;

            if (vh.Photo.Visibility == ViewStates.Visible)
            {
                if ((trip?.Photos?.Count).GetValueOrDefault() > 0)
                    Square.Picasso.Picasso.With(activity).Load($"file://{trip.Photos[0].PhotoUrl}").Into(vh.Photo);
                else
                    Square.Picasso.Picasso.With(activity).Load(trip.MainPhotoUrl).Into(vh.Photo);
            }
        }

        void OnClick(TripClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        void OnClickLong(TripClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }
}