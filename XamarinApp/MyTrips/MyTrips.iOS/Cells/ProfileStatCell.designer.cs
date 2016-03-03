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

namespace MyTrips.iOS
{
    [Register ("ProfileStatCell")]
    partial class ProfileStatCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblStatName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblText { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblStatName != null) {
                lblStatName.Dispose ();
                lblStatName = null;
            }

            if (lblText != null) {
                lblText.Dispose ();
                lblText = null;
            }
        }
    }
}