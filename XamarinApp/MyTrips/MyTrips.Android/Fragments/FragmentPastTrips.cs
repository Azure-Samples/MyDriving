using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using Android.Widget;
using MyTrips.ViewModel;
using System;
using MyTrips.Droid.Activities;
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

            adapter = new TripAdapter(viewModel);
            adapter.ItemClick += OnItemClick;
            layoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.SetAdapter(adapter);

            return view;
        }


        public override void OnStart()
        {
            base.OnStart();
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            if(viewModel.Trips.Count == 0)
                viewModel.LoadPastTripsCommand.Execute(null);
        }

        void ViewModel_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
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

        void OnItemClick (object sender, int position)
        {
            var trip = viewModel.Trips[position];
            var intent = new Intent(Activity, typeof(PastTripDetailsActivity));
            intent.PutExtra(nameof(trip.Id), trip.Id);
            StartActivity(intent);
        }
    }


    public class TripViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title {get;set;}
        public TextView Subtitle {get;set;}

        public TripViewHolder(View itemView, Action<int> listener) : base (itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.text_title);
            Subtitle = itemView.FindViewById<TextView>(Resource.Id.text_subtitle);
            itemView.Click += (sender, e) => listener (AdapterPosition);
        }
    }

    public class TripAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;

        PastTripsViewModel viewModel;
        public TripAdapter(PastTripsViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.viewModel.Trips.CollectionChanged += (sender, e) => NotifyDataSetChanged();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
            new TripViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_trip, parent, false), OnClick);

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as TripViewHolder;
            if (vh == null)
                return;

            var trip = viewModel.Trips[position];
            vh.Title.Text = trip.TripId;
            vh.Subtitle.Text = trip.TotalDistance;
        }

        public override int ItemCount => viewModel.Trips.Count;

        void OnClick (int position)
        {
            if (ItemClick != null)
                ItemClick (this, position);
        }
    }


}