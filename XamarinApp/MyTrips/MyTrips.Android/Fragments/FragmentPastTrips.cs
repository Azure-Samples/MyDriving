using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using Android.Widget;
using MyTrips.ViewModel;
using System;
using MyTrips.Droid.Activities;
using MyTrips.Droid.Controls;
using Android.Content;



namespace MyTrips.Droid.Fragments
{
    public class FragmentPastTrips : Fragment
    {

        public static FragmentPastTrips NewInstance() => new FragmentPastTrips { Arguments = new Bundle() };

        RecyclerView recyclerView;
        SwipeRefreshLayout refresher;
        TripAdapter adapter;
        PastTripsViewModel viewModel;
        LinearLayoutManager layoutManager;
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
            layoutManager = new LinearLayoutManager(Activity);
            layoutManager.Orientation = LinearLayoutManager.Vertical;
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.SetAdapter(adapter);
            recyclerView.ClearOnScrollListeners();
            recyclerView.AddOnScrollListener(new TripsOnScrollListenerListener(viewModel, layoutManager));

            return view;
        }

        class TripsOnScrollListenerListener : RecyclerView.OnScrollListener
        {
            PastTripsViewModel viewModel;
            LinearLayoutManager layoutManager;
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

        public async override void OnStart()
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
            var intent = new Intent(Activity, typeof(PastTripDetailsActivity));
            intent.PutExtra(nameof(trip.Id), trip.Id);
            intent.PutExtra(nameof(trip.Rating), trip.Rating);
            Activity.StartActivity(intent);
        }

        async void OnItemLongClick(object sender, TripClickEventArgs args)
        {
            var trip = viewModel.Trips[args.Position];
            await viewModel.ExecuteDeleteTripCommand(trip);
        }
    }


    public class TripViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; set; }
        public TextView Date { get; set; }
        public TextView Distance { get; set; }
        public ImageView Photo { get; set; }

        public TripViewHolder(View itemView, Action<TripClickEventArgs> listener, Action<TripClickEventArgs> listenerLong) : base(itemView)
        {
            itemView.LongClickable = true;
            Title = itemView.FindViewById<TextView>(Resource.Id.text_title);
            Distance = itemView.FindViewById<TextView>(Resource.Id.text_distance);
            Date = itemView.FindViewById<TextView>(Resource.Id.text_date);
            Photo = itemView.FindViewById<ImageView>(Resource.Id.photo);
            itemView.Click += (sender, e) => listener(new TripClickEventArgs { View = sender as View, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => listenerLong(new TripClickEventArgs { View = sender as View, Position = AdapterPosition });
        }
    }

    public class TripClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }

    public class TripAdapter : RecyclerView.Adapter
    {
        
        public event EventHandler<TripClickEventArgs> ItemClick;
        public event EventHandler<TripClickEventArgs> ItemLongClick;
        PastTripsViewModel viewModel;
        Android.App.Activity activity;
        public TripAdapter(Android.App.Activity activity, PastTripsViewModel viewModel)
        {
            this.activity = activity;
            this.viewModel = viewModel;

            this.viewModel.Trips.CollectionChanged += (sender, e) =>
            {
                this.activity.RunOnUiThread(() => 
                {
                    NotifyDataSetChanged();
                });
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
            new TripViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_trip, parent, false), OnClick, OnClickLong);

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as TripViewHolder;
            if (vh == null)
                return;

            var trip = viewModel.Trips[position];
            vh.Title.Text = trip.Name;
            vh.Distance.Text = trip.TotalDistance;
            vh.Date.Text = trip.TimeAgo;
            vh.Photo.Visibility = (trip?.Photos?.Count).GetValueOrDefault() > 0 || !string.IsNullOrWhiteSpace(trip.MainPhotoUrl) ? ViewStates.Visible : ViewStates.Gone;

            if (vh.Photo.Visibility == ViewStates.Visible)
            {
                if ((trip?.Photos?.Count).GetValueOrDefault() > 0)
                    Square.Picasso.Picasso.With(activity).Load($"file://{trip.Photos[0].PhotoUrl}").Into(vh.Photo);
                else
                    Square.Picasso.Picasso.With(activity).Load(trip.MainPhotoUrl).Into(vh.Photo);
            }

        }

        public override int ItemCount => viewModel.Trips.Count;

        void OnClick(TripClickEventArgs args)
        {
            if (ItemClick != null)
                ItemClick(this, args);
        }

        void OnClickLong(TripClickEventArgs args)
        {
            if (ItemLongClick != null)
                ItemLongClick(this, args);
        }
    }


}