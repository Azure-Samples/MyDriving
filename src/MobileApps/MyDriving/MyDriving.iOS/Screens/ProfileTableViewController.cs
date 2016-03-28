// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Foundation;
using System.Collections.Generic;
using UIKit;
using System;
using MyDriving.ViewModel;
using SDWebImage;
using System.Threading.Tasks;

namespace MyDriving.iOS
{
    public partial class ProfileTableViewController : UITableViewController
    {
        const string StatCellIdentifier = "STAT_CELL_IDENTIFIER";
        List<DrivingStatistic> data;

        public ProfileTableViewController(IntPtr handle) : base(handle)
        {
        }

        public ProfileViewModel ViewModel { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel = new ProfileViewModel();
            NavigationItem.Title = $"{ViewModel.Settings.UserFirstName} {ViewModel.Settings.UserLastName}";

            var url = ViewModel.Settings.UserProfileUrl;
            imgAvatar.SetImage(new NSUrl(url));

            imgAvatar.Layer.CornerRadius = imgAvatar.Frame.Width/2;
            imgAvatar.Layer.BorderWidth = 2;
            imgAvatar.Layer.BorderColor = "15A9FE".ToUIColor().CGColor;
            imgAvatar.Layer.MasksToBounds = true;

            UpdateUI();
        }

        public override async void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            await ViewModel.UpdateProfileAsync()
                .ContinueWith(t => { UpdateUI(); }, scheduler: TaskScheduler.FromCurrentSynchronizationContext());
        }

        void UpdateUI()
        {
            lblDrivingSkills.Text = $"Driving Skills: {ViewModel.DrivingSkillsPlacementBucket.Description}";
			lblScore.Text = $"Score: {ViewModel.DrivingSkills}%";
            PercentageView.Value = (ViewModel.DrivingSkills/100f)*360f;
            data = new List<DrivingStatistic>
            {
                new DrivingStatistic {Name = "Total Distance", Value = ViewModel.TotalDistanceDisplay},
                new DrivingStatistic {Name = "Total Duration", Value = ViewModel.TotalTimeDisplay},
                new DrivingStatistic {Name = "Max Speed", Value = ViewModel.MaxSpeedDisplay},
                new DrivingStatistic {Name = "Fuel Consumption", Value = ViewModel.FuelDisplay},
                new DrivingStatistic {Name = "Hard Stops", Value = ViewModel.HardStops.ToString()},
                new DrivingStatistic {Name = "Hard Accelerations", Value = ViewModel.HardAccelerations.ToString()},
                new DrivingStatistic {Name = "Total Trips", Value = ViewModel.TotalTrips.ToString()},
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
            var cell = tableView.DequeueReusableCell(StatCellIdentifier) as ProfileStatCell;

            cell.Name = data[indexPath.Row].Name;
            cell.Value = data[indexPath.Row].Value;

            return cell;
        }
        #endregion
    }
}