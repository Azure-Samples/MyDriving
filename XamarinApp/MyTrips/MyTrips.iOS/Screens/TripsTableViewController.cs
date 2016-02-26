using Foundation;
using System;
using UIKit;

using MyTrips.ViewModel;

using Humanizer;

namespace MyTrips.iOS
{
    public partial class TripsTableViewController : UITableViewController
    {
		const string TRIP_CELL_IDENTIFIER = "TRIP_CELL_IDENTIFIER";

		PastTripsViewModel ViewModel { get; set; }

        public TripsTableViewController (IntPtr handle) : base (handle)
        {
			
        }

		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();

			ViewModel = new PastTripsViewModel();
			await ViewModel.ExecuteLoadPastTripsCommandAsync();
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "pastTripSegue")
			{
				var controller = segue.DestinationViewController as CurrentTripViewController;
				var indexPath = TableView.IndexPathForCell(sender as UITableViewCell);
				var trip = ViewModel.Trips[indexPath.Row];

				controller.PastTripsDetailViewModel = new PastTripsDetailViewModel(trip);
			}
		}

		#region UITableViewSource
		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return ViewModel.Trips.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(TRIP_CELL_IDENTIFIER) as TripTableViewCell;

			if (cell == null)
			{
				cell = new TripTableViewCell(new NSString(TRIP_CELL_IDENTIFIER));
			}

			var trip = ViewModel.Trips[indexPath.Row];
            cell.LocationName = trip.TripId;
            cell.TimeAgo = trip.TimeAgo;
			cell.Distance = trip.TotalDistance;

			return cell;
		}
		#endregion
    }
}