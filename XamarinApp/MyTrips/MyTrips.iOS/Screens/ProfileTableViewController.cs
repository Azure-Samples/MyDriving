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

			imgAvatar.Layer.CornerRadius = imgAvatar.Frame.Width / 2;
			imgAvatar.Layer.BorderWidth = 2;
			imgAvatar.Layer.BorderColor = "15A9FE".ToUIColor().CGColor;
			imgAvatar.Layer.MasksToBounds = true;		
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

			cell.Text = "Miles";
			cell.Value = "65,876 mi";

			return cell;
		}
		#endregion

    }
}