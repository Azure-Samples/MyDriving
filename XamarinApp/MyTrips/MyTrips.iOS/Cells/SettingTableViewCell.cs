using System;

using Foundation;
using UIKit;

namespace MyTrips.iOS
{
    public partial class SettingTableViewCell : UITableViewCell
    {
		public SettingTableViewCell(IntPtr handle) : base(handle) { }
		public SettingTableViewCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId) { }

		public string Name
		{
			get { return nameLabel.Text; }
			set { nameLabel.Text = value; }
		}

		public string Value
		{
			get { return valueLabel.Text; }
			set { valueLabel.Text = value; }
		}
    }
}