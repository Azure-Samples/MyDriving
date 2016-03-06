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
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
			//Prepare buttons for fade in animation.
			btnFacebook.Alpha = 0;
			btnTwitter.Alpha = 0;
			btnMicrosoft.Alpha = 0;
			btnSkipAuth.Alpha = 0;
		}

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(viewModel.IsLoggedIn):
                    if(viewModel.IsLoggedIn)
                        GoToMain();
                    break;
            }
        }

        public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

            if (Settings.Current.IsLoggedIn || viewModel.IsLoggedIn)
            {
                GoToMain();
                return;
            }

            if (didAnimate)
                return;

            didAnimate = true;
			btnFacebook.FadeIn(0.3, 0.3f);
			btnTwitter.FadeIn(0.3, 0.5f);
			btnMicrosoft.FadeIn(0.3, 0.7f);
			btnSkipAuth.FadeIn(0.3, 0.9f);
		}

		partial void BtnFacebook_TouchUpInside(UIButton sender) => LoginAsync(LoginAccount.Facebook);


        partial void BtnTwitter_TouchUpInside(UIButton sender) => LoginAsync(LoginAccount.Twitter);


        partial void BtnMicrosoft_TouchUpInside(UIButton sender) => LoginAsync(LoginAccount.Microsoft);


        void LoginAsync(LoginAccount account)
        {
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

        partial void BtnSkipAuth_TouchUpInside(UIButton sender) => GoToMain(true);

        void GoToMain(bool fakeUser = false)
        {
            if (fakeUser)
                viewModel.InitFakeUser();
            var app = (AppDelegate)UIApplication.SharedApplication.Delegate;
            var viewController = UIStoryboard.FromName("Main", null).InstantiateViewController("tabBarController") as UITabBarController;
            viewController.SelectedIndex = 1;
            app.Window.RootViewController = viewController;
        }
	}
}
