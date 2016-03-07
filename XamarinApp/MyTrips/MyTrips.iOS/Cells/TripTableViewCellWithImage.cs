using System;

using Foundation;
using UIKit;

namespace MyTrips.iOS
{
    public partial class TripTableViewCellWithImage : UITableViewCell
    {
		public TripTableViewCellWithImage(IntPtr handle) : base(handle) { }
		public TripTableViewCellWithImage(NSString cellId) : base(UITableViewCellStyle.Default, cellId) { }

		public UIImageView DisplayImage
		{
			get { return displayImageView; }
			set { displayImageView = value; }
		}

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