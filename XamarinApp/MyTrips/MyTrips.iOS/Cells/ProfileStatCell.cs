using Foundation;
using System;
using UIKit;

namespace MyTrips.iOS
{
    public partial class ProfileStatCell : UITableViewCell
    {
		public ProfileStatCell (IntPtr handle) : base (handle)
		{

		}

		public ProfileStatCell(NSString cellId)
			: base(UITableViewCellStyle.Default, cellId)
		{

		}

		public string Value
		{
			get
			{
				return lblValue.Text;
			}
			set
			{
				lblValue.Text = value;
			}
		}

		public string Name
		{
			get
			{
				return lblText.Text;
			}
			set
			{
				lblText.Text = value;
			}
		}
    }
}