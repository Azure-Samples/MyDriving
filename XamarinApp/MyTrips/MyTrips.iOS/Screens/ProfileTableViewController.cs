using Foundation;
using System.Collections.Generic;
using UIKit;
using System;

namespace MyTrips.iOS
{
    public partial class ProfileTableViewController : UITableViewController
    {
		const string STAT_CELL_IDENTIFIER = "STAT_CELL_IDENTIFIER";
		List<ProfileStat> data; 

        public ProfileTableViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			imgAvatar.Layer.CornerRadius = imgAvatar.Frame.Width / 2;
			imgAvatar.Layer.BorderWidth = 2;
			imgAvatar.Layer.BorderColor = "15A9FE".ToUIColor().CGColor;
			imgAvatar.Layer.MasksToBounds = true;

			data = new List<ProfileStat>
			{
				new ProfileStat { Name = "Distance", Value = "275.43 miles"},
				new ProfileStat { Name = "Time", Value = "8 hours, 45 minutes"},
				new ProfileStat { Name = "Average Speed", Value = "31 MPH"},
				new ProfileStat { Name = "Average Consumption", Value = "2.5 gallons"},
				new ProfileStat { Name = "Hard Breaks", Value = "21"},
				new ProfileStat { Name = "Tips Received", Value = "14"},
			};
		}


		#region UITableViewSource
		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return 6;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(STAT_CELL_IDENTIFIER) as ProfileStatCell;

			cell.Text = data[indexPath.Row].Name;
			cell.Value = data[indexPath.Row].Value;

			return cell;
		}
		#endregion

    }
}