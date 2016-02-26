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

			// TODO: Grab data for UITableView.
			ViewModel = new PastTripsViewModel();

			await ViewModel.ExecuteLoadPastTripsCommandAsync();
		}

		#region UITableViewSource
		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return ViewModel.Trips.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			// No need to check for null; storyboards always return a dequeable cell.
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