using Foundation;
using System.Collections.Generic;
using UIKit;
using System;
using MyTrips.ViewModel;
using SDWebImage;
using System.Threading.Tasks;

namespace MyTrips.iOS
{
    public partial class ProfileTableViewController : UITableViewController
    {
		const string STAT_CELL_IDENTIFIER = "STAT_CELL_IDENTIFIER";
		List<DrivingStatistic> data;

		public ProfileViewModel ViewModel { get; set; }

		public ProfileTableViewController(IntPtr handle) : base(handle) { }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			ViewModel = new ProfileViewModel();
			NavigationItem.Title = $"{ViewModel.Settings.UserFirstName} {ViewModel.Settings.UserLastName}";

			var url = ViewModel.Settings.UserProfileUrl;
			imgAvatar.SetImage(new NSUrl(url));

			
			imgAvatar.Layer.CornerRadius = imgAvatar.Frame.Width / 2;
			imgAvatar.Layer.BorderWidth = 2;
			imgAvatar.Layer.BorderColor = "15A9FE".ToUIColor().CGColor;
			imgAvatar.Layer.MasksToBounds = true;

            UpdateUI();
		}

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ViewModel.UpdateProfileAsync().ContinueWith((t) =>
            {
                UpdateUI();   
            }, scheduler: TaskScheduler.FromCurrentSynchronizationContext()); 
        }

        void UpdateUI()
        {
            PercentageView.Value = (ViewModel.DrivingSkills / 100f) * 360f;
            data = new List<DrivingStatistic>
            {
                new DrivingStatistic { Name = "Total Distance", Value = ViewModel.TotalDistanceDisplay},
                new DrivingStatistic { Name = "Total Duration", Value = ViewModel.TotalTimeDisplay },
                new DrivingStatistic { Name = "Max Speed", Value = ViewModel.MaxSpeedDisplay },
                new DrivingStatistic { Name = "Fuel Consumption", Value = ViewModel.FuelDisplay},
                new DrivingStatistic { Name = "Hard Breaks", Value = ViewModel.HardStops.ToString()},
                new DrivingStatistic { Name = "Hard Accelerations", Value = ViewModel.HardAccelerations.ToString()},
                new DrivingStatistic { Name = "Total Trips", Value = ViewModel.TotalTrips.ToString()},

            };

            TableView.ReloadData();
        }



		#region UITableViewSource
		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return 60;
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
            return (data?.Count).GetValueOrDefault();
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(STAT_CELL_IDENTIFIER) as ProfileStatCell;

			cell.Name = data[indexPath.Row].Name;
			cell.Value = data[indexPath.Row].Value;

			return cell;
		}
		#endregion
    }
}