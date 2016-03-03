using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MyTrips.ViewModel;
using MyTrips.Utils;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyTrips.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfileView : Page
    {
        ProfileViewModel profileViewModel;
        public ProfileView()
        {
            this.InitializeComponent();
            profileViewModel = new ProfileViewModel();
            profileViewModel.DrivingSkills = new System.Random().Next(0, 100);
            SetImageSource();
        }


        private void SetImageSource()
        {
            bool useByteArrForPicture = false;   //microsoft account gives a byte array not url for picture
            if (Settings.Current.UserProfileUrl == string.Empty && Settings.Current.LoginAccount == LoginAccount.Microsoft)
            {
                useByteArrForPicture = true;
            }

            if (useByteArrForPicture)
            {
                ProfileImage.ImageSource = Helpers.BitmapImageConverter.ConvertImage(Settings.Current.UserProfileByteArr);
            }
            else 
            {
                //use picture url
                ProfileImage.ImageSource = new BitmapImage(new Uri(Settings.Current.UserProfileUrl));
            }
        }

    }
}
