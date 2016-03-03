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

		public string StatName
		{
			get
			{
				return lblStatName.Text;
			}
			set
			{
				lblStatName.Text = value;
			}
		}

		public string Text
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