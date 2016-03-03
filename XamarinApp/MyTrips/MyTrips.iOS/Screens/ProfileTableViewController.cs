using Foundation;
using System;
using UIKit;

namespace MyTrips.iOS
{
    public partial class ProfileTableViewController : UITableViewController
    {
        public ProfileTableViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
		}


		#region UITableViewSource

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return 6;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell("STAT_CELL_IDENTIFIER") as ProfileStatCell;

			if (cell == null)
			{
				cell = new ProfileStatCell(new NSString("STAT_CELL_IDENTIFIER"));
			}

			cell.StatName = "Miles";
			cell.Text = "65,876 mi";

			return cell;
		}
		#endregion
    }
}