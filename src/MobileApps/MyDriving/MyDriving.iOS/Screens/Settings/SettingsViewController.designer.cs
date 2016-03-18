// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MyDriving.iOS
{
    [Register ("SettingsViewController")]
    partial class SettingsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnLogout { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView settingsTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnLogout != null) {
                btnLogout.Dispose ();
                btnLogout = null;
            }

            if (settingsTableView != null) {
                settingsTableView.Dispose ();
                settingsTableView = null;
            }
        }
    }
}