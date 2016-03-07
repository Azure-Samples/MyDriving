using Foundation;
using System;
using UIKit;

namespace MyTrips.iOS
{
    public partial class TripSummaryTableViewController : UIViewController
    {
        public TripSummaryTableViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();


		}

		partial void UIButtonmgbbGHnL_TouchUpInside(UIButton sender)
		{
			DismissViewController(true, null);
		}
	}
}