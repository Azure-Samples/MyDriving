using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MyTrips.iOS
{
    [Register ("SettingDetailTextFieldTableViewCell")]
    partial class SettingDetailTextFieldTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField settingTextField { get; set; }

        [Action ("SettingTextFieldValue_Changed:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SettingTextFieldValue_Changed (UIKit.UITextField sender);

        [Action ("EditingBegan:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void EditingBegan (UIKit.UITextField sender);

        [Action ("EditingEnded:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void EditingEnded (UIKit.UITextField sender);

        void ReleaseDesignerOutlets ()
        {
            if (settingTextField != null) {
                settingTextField.Dispose ();
                settingTextField = null;
            }
        }
    }
}