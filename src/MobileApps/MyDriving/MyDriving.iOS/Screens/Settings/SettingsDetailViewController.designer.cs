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
    [Register ("SettingsDetailViewController")]
    partial class SettingsDetailViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView settingsDetailTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (settingsDetailTableView != null) {
                settingsDetailTableView.Dispose ();
                settingsDetailTableView = null;
            }
        }
    }
}