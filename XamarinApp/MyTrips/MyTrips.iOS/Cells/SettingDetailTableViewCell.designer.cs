using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MyTrips.iOS
{
    [Register ("SettingDetailTableViewCell")]
    partial class SettingDetailTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel settingNameLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (settingNameLabel != null) {
                settingNameLabel.Dispose ();
                settingNameLabel = null;
            }
        }
    }
}