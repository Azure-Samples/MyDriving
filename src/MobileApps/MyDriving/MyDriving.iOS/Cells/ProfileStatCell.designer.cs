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
    [Register ("ProfileStatCell")]
    partial class ProfileStatCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblValue { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblText != null) {
                lblText.Dispose ();
                lblText = null;
            }

            if (lblValue != null) {
                lblValue.Dispose ();
                lblValue = null;
            }
        }
    }
}