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
			btnSkipAuth.Alpha = 0;

			btnSkipAuth.Layer.CornerRadius = 4;
			btnSkipAuth.Layer.MasksToBounds = true;
		}

		public override async void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

            if (didAnimate)
                return;

            didAnimate = true;
			btnFacebook.FadeIn(0.3, 0.3f);
			btnTwitter.FadeIn(0.3, 0.5f);
			btnMicrosoft.FadeIn(0.3, 0.7f);
			btnSkipAuth.FadeIn(0.3, 0.9f);
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
            switch (account)
            {
                case LoginAccount.Facebook:
					await viewModel.ExecuteLoginFacebookCommandAsync();
                    break;
                case LoginAccount.Microsoft:
					await viewModel.ExecuteLoginMicrosoftCommandAsync();
                    break;
                case LoginAccount.Twitter:
					await viewModel.ExecuteLoginTwitterCommandAsync();
                    break;
            }

			if (viewModel.IsLoggedIn)
				NavigateToTabs();
        }

		partial void BtnSkipAuth_TouchUpInside(UIButton sender)
		{
			viewModel.InitFakeUser();
			NavigateToTabs();
		}

		void NavigateToTabs()
		{
			InvokeOnMainThread(() =>
			{
				var app = (AppDelegate)UIApplication.SharedApplication.Delegate;
				var viewController = UIStoryboard.FromName("Main", null).InstantiateViewController("tabBarController") as UITabBarController;
				viewController.SelectedIndex = 1;
				app.Window.RootViewController = viewController;
			});
		}
	}
}
