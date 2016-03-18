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
    [Register ("LoginViewController")]
    partial class LoginViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnFacebook { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnMicrosoft { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSkipAuth { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnTwitter { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgLogo { get; set; }

        [Action ("BtnFacebook_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnFacebook_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnTwitter_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnTwitter_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnMicrosoft_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnMicrosoft_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnSkipAuth_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnSkipAuth_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnFacebook != null) {
                btnFacebook.Dispose ();
                btnFacebook = null;
            }

            if (btnMicrosoft != null) {
                btnMicrosoft.Dispose ();
                btnMicrosoft = null;
            }

            if (btnSkipAuth != null) {
                btnSkipAuth.Dispose ();
                btnSkipAuth = null;
            }

            if (btnTwitter != null) {
                btnTwitter.Dispose ();
                btnTwitter = null;
            }

            if (imgLogo != null) {
                imgLogo.Dispose ();
                imgLogo = null;
            }
        }
    }
}