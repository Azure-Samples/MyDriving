using Foundation;
using System;
using UIKit;

namespace MyTrips.iOS
{
    public partial class TripTableViewCell : UITableViewCell
    {
        public TripTableViewCell (IntPtr handle) : base (handle)
        {
        }

		public TripTableViewCell(NSString cellId)
			: base(UITableViewCellStyle.Default, cellId)
		{
		}

    }
}