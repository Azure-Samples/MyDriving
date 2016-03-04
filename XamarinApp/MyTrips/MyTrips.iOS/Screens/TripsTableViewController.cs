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

        public TripsTableViewController (IntPtr handle) : base (handle)
        {
			
        }

		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();

			ViewModel = new PastTripsViewModel();
			await ViewModel.ExecuteLoadPastTripsCommandAsync();

			TableView.TableFooterView = new UIView(new CGRect(0, 0, 0,0));
            TableView.ReloadData();

			// Check to see if 3D Touch is available
			if (TraitCollection.ForceTouchCapability == UIForceTouchCapability.Available)
				RegisterForPreviewingWithDelegate(new PreviewingDelegate(this), View);

			RefreshControl.AddTarget(this, new ObjCRuntime.Selector("RefreshSource"), UIControlEvent.ValueChanged);

			NSNotificationCenter.DefaultCenter.AddObserver(new NSString ("RefreshPastTripsTable"), HandleReloadTableNotification); 
		}

		public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
		{
			base.TraitCollectionDidChange(previousTraitCollection);

			// Check to see if 3D Touch is available
			if (TraitCollection.ForceTouchCapability == UIForceTouchCapability.Available)
				RegisterForPreviewingWithDelegate(new PreviewingDelegate(this), View);
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

				cell.LocationName = trip.TripId;
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
				cell.LocationName = trip.TripId;
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

	public class PreviewingDelegate : UIViewControllerPreviewingDelegate
	{

		PastTripsViewModel ViewModel;

		#region Computed Properties
		public TripsTableViewController MasterController { get; set; }
		#endregion

		#region Constructors
		public PreviewingDelegate (TripsTableViewController masterController)
		{
			// Initialize
			this.MasterController = masterController;
			ViewModel = masterController.ViewModel;
		}

		public PreviewingDelegate (NSObjectFlag t) : base(t)
		{
		}

		public PreviewingDelegate (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		/// Present the view controller for the "Pop" action.
		public override void CommitViewController (IUIViewControllerPreviewing previewingContext, UIViewController viewControllerToCommit)
		{
			// Reuse Peek view controller for details presentation
			MasterController.ShowViewController(viewControllerToCommit,this);
		}

		/// Create a previewing view controller to be shown at "Peek".
		public override UIViewController GetViewControllerForPreview (IUIViewControllerPreviewing previewingContext, CGPoint location)
		{
			// Grab the item to preview
			var indexPath = MasterController.TableView.IndexPathForRowAtPoint (location);
			var cell = MasterController.TableView.CellAt (indexPath);
			var item = MasterController.ViewModel.Trips[indexPath.Row];

			// Grab a controller and set it to the default sizes
			var detailViewController = MasterController.Storyboard.InstantiateViewController ("CURRENT_TRIP_STORYBIARD_IDENTIFIER") as CurrentTripViewController;
			detailViewController.PreferredContentSize = new CGSize (0, 0);

			// Set the data for the display
			var trip = ViewModel.Trips[indexPath.Row];
			detailViewController.PastTripsDetailViewModel = new PastTripsDetailViewModel(trip);

			detailViewController.NavigationItem.LeftBarButtonItem = MasterController.SplitViewController.DisplayModeButtonItem;
			detailViewController.NavigationItem.LeftItemsSupplementBackButton = true;

			// Set the source rect to the cell frame, so everything else is blurred.
			previewingContext.SourceRect = cell.Frame;

			return detailViewController;
		}
		#endregion
	}
}