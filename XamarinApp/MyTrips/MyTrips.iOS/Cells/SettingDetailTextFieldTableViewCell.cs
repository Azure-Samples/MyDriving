using System;

using Foundation;
using UIKit;

namespace MyTrips.iOS
{
	public partial class SettingDetailTextFieldTableViewCell : UITableViewCell
    {
		public SettingDetailTextFieldTableViewCell (IntPtr handle) : base (handle) { }
		public SettingDetailTextFieldTableViewCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId) { }

		public string Row { get; set; }
		public string Section { get; set; }

		public string Value
		{
			get { return settingTextField.Text; }
			set { settingTextField.Text = value; }
		}

		partial void EditingEnded(UITextField sender)
		{
			sender.ResignFirstResponder();
		}

		partial void EditingBegan(UITextField sender)
		{
			sender.BecomeFirstResponder();
		}

		partial void SettingTextFieldValue_Changed(UITextField sender)
		{
			if (sender != null && !String.IsNullOrEmpty(sender.Text))
			{
				var dict = new NSDictionary("Value", sender.Text, "Row", Row, "Section", Section);
				NSNotificationCenter.DefaultCenter.PostNotificationName("SettingTextFieldChanged", dict);
			}		
		}
	}
}