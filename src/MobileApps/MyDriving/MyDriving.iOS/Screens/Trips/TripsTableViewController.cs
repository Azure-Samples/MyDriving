// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;
using MyDriving.ViewModel;
using SDWebImage;

namespace MyDriving.iOS
{
    public partial class TripsTableViewController : UITableViewController
    {
        const string TripCellWithimageIdentifier = "TRIP_CELL_WITHIMAGE_IDENTIFIER";
        const string PastTripSegueIdentifier = "pastTripSegue";

        public TripsTableViewController(IntPtr handle) : base(handle)
        {
        }

        public PastTripsViewModel ViewModel { get; set; }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel = new PastTripsViewModel();
            await ViewModel.ExecuteLoadPastTripsCommandAsync();

            TableView.ReloadData();
            TableView.TableFooterView = new UIView(new CGRect(0, 0, 0, 0));

            RefreshControl.AddTarget(this, new Selector("RefreshSource"), UIControlEvent.ValueChanged);
            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("RefreshPastTripsTable"),
                HandleReloadTableNotification);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == PastTripSegueIdentifier)
            {
                var controller = segue.DestinationViewController as CurrentTripViewController;
                var indexPath = TableView.IndexPathForCell(sender as UITableViewCell);
                var trip = ViewModel.Trips[indexPath.Row];

                controller.PastTripsDetailViewModel = new PastTripsDetailViewModel(trip);
            }
        }

        [Export("RefreshSource")]
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

            InvokeOnMainThread(delegate { TableView.ReloadData(); });
        }

        #region UITableViewSource

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return ViewModel.Trips.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(TripCellWithimageIdentifier) as TripTableViewCellWithImage ??
                       new TripTableViewCellWithImage(new NSString(TripCellWithimageIdentifier));

            var trip = ViewModel.Trips[indexPath.Row];
            cell.DisplayImage.SetImage(new NSUrl(trip.MainPhotoUrl));
            cell.LocationName = trip.Name;
            cell.TimeAgo = trip.TimeAgo;
            cell.Distance = trip.TotalDistance;
            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 221;
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        public override async void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle,
            NSIndexPath indexPath)
        {
            switch (editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    var trip = ViewModel.Trips[indexPath.Row];
                    await ViewModel.ExecuteDeleteTripCommand(trip);

                    tableView.DeleteRows(new[] {indexPath}, UITableViewRowAnimation.Automatic);
                    break;
            }
        }

        #endregion
    }
}