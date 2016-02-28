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

			//UpdateHeader(64);
		}

		void UpdateHeader(int percentage)
		{
			lblPercentage.Text = $"{percentage}%";
			lblBetterThan.Text = $"Better driver than {percentage} of Americans";

			var defaultAttributes = new UIStringAttributes {
				ForegroundColor = lblBetterThan.TextColor,
			};

			var hitAttributes = new UIStringAttributes {
				ForegroundColor = Colors.PRIMARY,
			};

			var attributedString = new NSMutableAttributedString (lblBetterThan.Text);
			attributedString.SetAttributes(defaultAttributes.Dictionary, new NSRange(0, lblBetterThan.Text.Length));
			attributedString.SetAttributes(hitAttributes.Dictionary, new NSRange(15, lblBetterThan.Text.Length));

			lblBetterThan.AttributedText = attributedString;
		}
    }
}