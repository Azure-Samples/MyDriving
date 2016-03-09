using System;

using CoreGraphics;
using Foundation;
using UIKit;

using MyTrips.ViewModel;
using SDWebImage;

namespace MyTrips.iOS
{
    public partial class TripsTableViewController : UITableViewController
    {
		const string TRIP_CELL_IDENTIFIER = "TRIP_CELL_IDENTIFIER";
		const string TRIP_CELL_WITHIMAGE_IDENTIFIER = "TRIP_CELL_WITHIMAGE_IDENTIFIER";
		const string PAST_TRIP_SEGUE_IDENTIFIER = "pastTripSegue";

		public PastTripsViewModel ViewModel { get; set; }

		public TripsTableViewController(IntPtr handle) : base(handle) { }

		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();

			ViewModel = new PastTripsViewModel();
			await ViewModel.ExecuteLoadPastTripsCommandAsync();

			TableView.TableFooterView = new UIView(new CGRect(0, 0, 0,0));
            TableView.ReloadData();

			RefreshControl.AddTarget(this, new ObjCRuntime.Selector("RefreshSource"), UIControlEvent.ValueChanged);
			NSNotificationCenter.DefaultCenter.AddObserver(new NSString ("RefreshPastTripsTable"), HandleReloadTableNotification); 
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == PAST_TRIP_SEGUE_IDENTIFIER)
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
			var trip = ViewModel.Trips[indexPath.Row];

			if (String.IsNullOrEmpty(trip.MainPhotoUrl))
			{
				var cell = tableView.DequeueReusableCell(TRIP_CELL_IDENTIFIER) as TripTableViewCell;

				if (cell == null)
				{
					cell = new TripTableViewCell(new NSString(TRIP_CELL_IDENTIFIER));
				}

				cell.LocationName = trip.Name;
				cell.TimeAgo = trip.TimeAgo;
				cell.Distance = trip.TotalDistance;

				return cell;
			}
			else
			{
				var cell = tableView.DequeueReusableCell(TRIP_CELL_WITHIMAGE_IDENTIFIER) as TripTableViewCellWithImage;

				if (cell == null)
				{
					cell = new TripTableViewCellWithImage(new NSString(TRIP_CELL_WITHIMAGE_IDENTIFIER));
				}

				cell.DisplayImage.SetImage(new NSUrl(trip.MainPhotoUrl));
				cell.LocationName = trip.Name;
				cell.TimeAgo = trip.TimeAgo;
				cell.Distance = trip.TotalDistance;
				return cell;
			}
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			var mainPhotoUrl = ViewModel.Trips[indexPath.Row].MainPhotoUrl;
			if (string.IsNullOrEmpty(mainPhotoUrl))
			{
				return 60;
			}
			else
			{
				return 221;
			}
		}
		#endregion

		// For some reason, RefreshControl.ValueChanged doesn't function properly?
		[Export ("RefreshSource")]
		async void RefreshControl_ValueChanged()
		{
			await ViewModel.ExecuteLoadPastTripsCommandAsync();

			InvokeOnMainThread(delegate
			{
				TableView.ReloadData();
				RefreshControl.EndRefreshing();
			});
		}

		async void HandleReloadTableNotification(NSNotification obj)
		{
			await ViewModel.ExecuteLoadPastTripsCommandAsync();

			InvokeOnMainThread(delegate
			{
				TableView.ReloadData();
			});
		}
	}
}