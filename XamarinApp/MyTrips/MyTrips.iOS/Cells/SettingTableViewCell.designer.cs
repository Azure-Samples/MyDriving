using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MyTrips.iOS
{
    [Register ("SettingTableViewCell")]
    partial class SettingTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel nameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel valueLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (nameLabel != null) {
                nameLabel.Dispose ();
                nameLabel = null;
            }

            if (valueLabel != null) {
                valueLabel.Dispose ();
                valueLabel = null;
            }
        }
    }
}