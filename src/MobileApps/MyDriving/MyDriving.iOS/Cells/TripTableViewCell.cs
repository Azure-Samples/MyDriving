using Foundation;
using System;
using UIKit;

namespace MyDriving.iOS
{
    public partial class TripTableViewCell : UITableViewCell
    {
		public TripTableViewCell(IntPtr handle) : base(handle) { }
		public TripTableViewCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId) { }

		public string LocationName
		{
			get { return lblTitle.Text; }
			set { lblTitle.Text = value; }
		}

		public string TimeAgo
		{
			get { return lblDaysAgo.Text; }
			set { lblDaysAgo.Text = value; }
		}

		public string Distance
		{
			get { return lblMiles.Text; }
			set { lblMiles.Text = value; }
		}
    }
}