using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MyTrips.iOS
{
	partial class LoginViewController : UIViewController
	{
		public LoginViewController (IntPtr handle) : base (handle)
		{
			
		}

		public override void ViewDidLoad()
		{
			//Prepare buttons for fade in animation.
			btnFacebook.Alpha = 0;
			btnTwitter.Alpha = 0;
			btnMicrosoft.Alpha = 0;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			btnFacebook.FadeIn(0.3, 0.3f);
			btnTwitter.FadeIn(0.3, 0.5f);
			btnMicrosoft.FadeIn(0.3, 0.7f);
		}

		partial void BtnFacebook_TouchUpInside(UIButton sender)
		{
			throw new NotImplementedException();
		}

		partial void BtnTwitter_TouchUpInside(UIButton sender)
		{
			throw new NotImplementedException();
		}

		partial void BtnMicrosoft_TouchUpInside(UIButton sender)
		{
			throw new NotImplementedException();
		}
	}
}
