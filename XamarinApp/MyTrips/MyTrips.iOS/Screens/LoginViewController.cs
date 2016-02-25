using Foundation;
using MyTrips.Utils;
using MyTrips.ViewModel;
using System;
using System.CodeDom.Compiler;
using System.Threading.Tasks;
using UIKit;

namespace MyTrips.iOS
{
	partial class LoginViewController : UIViewController
	{
        LoginViewModel viewModel;
        bool didAnimate;
		public LoginViewController (IntPtr handle) : base (handle)
		{
			
		}

		public override void ViewDidLoad()
		{
            viewModel = new LoginViewModel();
			//Prepare buttons for fade in animation.
			btnFacebook.Alpha = 0;
			btnTwitter.Alpha = 0;
			btnMicrosoft.Alpha = 0;
		}

		public override async void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

            if (Settings.Current.IsLoggedIn)
            {
                await DismissViewControllerAsync(true);
                return;
            }

            if (didAnimate)
                return;

            didAnimate = true;
			btnFacebook.FadeIn(0.3, 0.3f);
			btnTwitter.FadeIn(0.3, 0.5f);
			btnMicrosoft.FadeIn(0.3, 0.7f);
		}

		async partial void BtnFacebook_TouchUpInside(UIButton sender)
		{
            await LoginAsync(LoginAccount.Facebook);
        }

		async partial void BtnTwitter_TouchUpInside(UIButton sender)
		{
            await LoginAsync(LoginAccount.Twitter);
        }

		async partial void BtnMicrosoft_TouchUpInside(UIButton sender)
		{
            await LoginAsync(LoginAccount.Microsoft);
		}

        async Task LoginAsync(LoginAccount account)
        {
#if DEBUG
            await DismissViewControllerAsync(true);
#endif

            switch (account)
            {
                case LoginAccount.Facebook:
                    viewModel.LoginFacebookCommand.Execute(null);
                    break;
                case LoginAccount.Microsoft:
                    viewModel.LoginMicrosoftCommand.Execute(null);
                    break;
                case LoginAccount.Twitter:
                    viewModel.LoginTwitterCommand.Execute(null);
                    break;
            }
        }
	}
}
