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
        TripAdapter _adapter;
        LinearLayoutManager _layoutManager;

        RecyclerView _recyclerView;
        SwipeRefreshLayout _refresher;
        PastTripsViewModel _viewModel;

        public static FragmentPastTrips NewInstance() => new FragmentPastTrips {Arguments = new Bundle()};

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_past_trips, null);

            _viewModel = new PastTripsViewModel();

            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _refresher = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);

            _refresher.Refresh += (sender, e) => _viewModel.LoadPastTripsCommand.Execute(null);


            _adapter = new TripAdapter(Activity, _viewModel);
            _adapter.ItemClick += OnItemClick;
            _adapter.ItemLongClick += OnItemLongClick;
            _layoutManager = new LinearLayoutManager(Activity) {Orientation = LinearLayoutManager.Vertical};
            _recyclerView.SetLayoutManager(_layoutManager);
            _recyclerView.SetAdapter(_adapter);
            _recyclerView.ClearOnScrollListeners();
            _recyclerView.AddOnScrollListener(new TripsOnScrollListenerListener(_viewModel, _layoutManager));

            return view;
        }

        public override async void OnStart()
        {
            base.OnStart();
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            if (_viewModel.Trips.Count == 0)
                await _viewModel.ExecuteLoadPastTripsCommandAsync();
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_viewModel.IsBusy):
                    _refresher.Refreshing = _viewModel.IsBusy;
                    break;
            }
        }

        public override void OnStop()
        {
            base.OnStop();
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        void OnItemClick(object sender, TripClickEventArgs args)
        {
            var trip = _viewModel.Trips[args.Position];
            var intent = new Intent(Activity, typeof (PastTripDetailsActivity));
            intent.PutExtra("Id", trip.Id);
            intent.PutExtra("Rating", trip.Rating);
            Activity.StartActivity(intent);
        }

        async void OnItemLongClick(object sender, TripClickEventArgs args)
        {
            var trip = _viewModel.Trips[args.Position];
            await _viewModel.ExecuteDeleteTripCommand(trip);
        }

        class TripsOnScrollListenerListener : RecyclerView.OnScrollListener
        {
            readonly LinearLayoutManager _layoutManager;
            readonly PastTripsViewModel _viewModel;

            public TripsOnScrollListenerListener(PastTripsViewModel viewModel, LinearLayoutManager layoutManager)
            {
                _layoutManager = layoutManager;
                _viewModel = viewModel;
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                base.OnScrolled(recyclerView, dx, dy);
                if (_viewModel.IsBusy || _viewModel.Trips.Count == 0 || !_viewModel.CanLoadMore)
                    return;

                var lastVisiblePosition = _layoutManager.FindLastCompletelyVisibleItemPosition();
                if (lastVisiblePosition == RecyclerView.NoPosition)
                    return;

                //if we are at the bottom and can load more.
                if (lastVisiblePosition == _viewModel.Trips.Count - 1)
                    _viewModel.LoadMorePastTripCommand.Execute(null);
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
        readonly Android.App.Activity _activity;
        readonly PastTripsViewModel _viewModel;

        public TripAdapter(Android.App.Activity activity, PastTripsViewModel viewModel)
        {
            _activity = activity;
            _viewModel = viewModel;

            _viewModel.Trips.CollectionChanged +=
                (sender, e) => { _activity.RunOnUiThread(NotifyDataSetChanged); };
        }

        public override int ItemCount => _viewModel.Trips.Count;

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

            var trip = _viewModel.Trips[position];
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
                    Square.Picasso.Picasso.With(_activity).Load($"file://{trip.Photos[0].PhotoUrl}").Into(vh.Photo);
                else
                    Square.Picasso.Picasso.With(_activity).Load(trip.MainPhotoUrl).Into(vh.Photo);
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