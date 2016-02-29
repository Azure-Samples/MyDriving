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

			UpdateHeader(64);
		}

		void UpdateHeader(int percentage)
		{
			lblPercentage.Text = $"{percentage}%";
			lblBetterThan.Text = $"Better driver than {percentage}% of Americans.";

			var defaultAttributes = new UIStringAttributes {
				ForegroundColor = lblDrivingSkills.TextColor,
			};

			var hitAttributes = new UIStringAttributes {
				ForegroundColor = Colors.PRIMARY,
			};

			/*
			var attributedString = new NSMutableAttributedString(lblDrivingSkills.Text);
			attributedString.SetAttributes(defaultAttributes.Dictionary, new NSRange(0, lblDrivingSkills.Text.Length));
			attributedString.SetAttributes(hitAttributes.Dictionary, new NSRange(15, lblDrivingSkills.Text.Length));

			lblDrivingSkills.AttributedText = attributedString;
			*/
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
			cell.SideColor = UIColor.FromRGB(77, 133, 202);

			return cell;
		}
		#endregion
    }
}