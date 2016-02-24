using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using Android.Widget;
using MyTrips.ViewModel;


namespace MyTrips.Droid.Fragments
{
    public class FragmentPastTrips : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static FragmentPastTrips NewInstance()
        {
            var frag1 = new FragmentPastTrips { Arguments = new Bundle() };
            return frag1;
        }

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

            refresher.Refresh += (sender, e) => 
                viewModel.LoadPastTripsCommand.Execute(null);

            adapter = new TripAdapter(viewModel);
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
    }


    public class TripViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title {get;set;}
        public TextView Subtitle {get;set;}

        public TripViewHolder(View itemView) : base (itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.text_title);
            Subtitle = itemView.FindViewById<TextView>(Resource.Id.text_subtitle);
        }
    }

    public class TripAdapter : RecyclerView.Adapter
    {
        
        PastTripsViewModel viewModel;
        public TripAdapter(PastTripsViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.viewModel.Trips.CollectionChanged += (sender, e) => NotifyDataSetChanged();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
            new TripViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_trip, parent, false));

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

    }


}