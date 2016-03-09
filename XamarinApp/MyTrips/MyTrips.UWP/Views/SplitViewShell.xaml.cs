using MyTrips.UWP.Controls;
using System;
using Windows.UI;
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
    public sealed partial class SplitViewShell : Page
    {
        Color selectedTextColor = Color.FromArgb(0xFF, 0x1b, 0xa0, 0xe1);
        SplitViewButtonContent selectedControl = null;
        public SplitViewShell(Frame frame)
        {
            this.InitializeComponent();
            this.MyTripsSplitView.Content = frame;
            frame.Navigated += Frame_Navigated;

            this.Current.labelText = "Current";
            this.Current.defaultImageSource = new BitmapImage(new Uri("ms-appx:///Assets/SplitView/default_current.png", UriKind.Absolute));
            this.Current.selectedImageSource = new BitmapImage(new Uri("ms-appx:///Assets/SplitView/selected_current.png", UriKind.Absolute));

            this.PastTrips.labelText = "Past Trips";
            this.PastTrips.defaultImageSource = new BitmapImage(new Uri("ms-appx:///Assets/SplitView/default_pastTrips.png", UriKind.Absolute));
            this.PastTrips.selectedImageSource = new BitmapImage(new Uri("ms-appx:///Assets/SplitView/selected_pastTrips.png", UriKind.Absolute));

            this.Profile.labelText = "Profile";
            this.Profile.defaultImageSource = new BitmapImage(new Uri("ms-appx:///Assets/SplitView/default_profile.png", UriKind.Absolute));
            this.Profile.selectedImageSource = new BitmapImage(new Uri("ms-appx:///Assets/SplitView/selected_profile.png", UriKind.Absolute));

            this.Settings.labelText = "Settings";
            this.Settings.defaultImageSource = new BitmapImage(new Uri("ms-appx:///Assets/SplitView/default_settings.png", UriKind.Absolute));
            this.Settings.selectedImageSource = new BitmapImage(new Uri("ms-appx:///Assets/SplitView/selected_settings.png", UriKind.Absolute));
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MyTripsSplitView.IsPaneOpen = !MyTripsSplitView.IsPaneOpen;
        }


        private void TripsButton_Click(object sender, RoutedEventArgs e)
        {
            SelectControl(PastTrips);
            MyTripsSplitView.IsPaneOpen = false;
            ((Frame)this.MyTripsSplitView.Content).Navigate(typeof(PastTripsMenuView));
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            SelectControl(Profile);
            MyTripsSplitView.IsPaneOpen = false;
            ((Frame)this.MyTripsSplitView.Content).Navigate(typeof(ProfileView));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SelectControl(Settings);
            MyTripsSplitView.IsPaneOpen = false;
            ((Frame)this.MyTripsSplitView.Content).Navigate(typeof(SettingsView));
        }

        private void NewTripButton_Click(object sender, RoutedEventArgs e)
        {
            SelectControl(Current);
            MyTripsSplitView.IsPaneOpen = false;
            ((Frame)this.MyTripsSplitView.Content).Navigate(typeof(CurrentTripView));
        }

        private void TripSummaryButton_Click(object sender, RoutedEventArgs e)
        {
            MyTripsSplitView.IsPaneOpen = false;

            //test code to load a trip
            //if (App.currentTrip == null)
            //{
            //    var trips = MyTrips.DataStore.Mock.Stores.TripStore.GetTrips();
            //    App.currentTrip = trips[4];
            //}
            //((Frame)this.MyTripsSplitView.Content).Navigate(typeof(TripSummaryView));
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            MyTripsSplitView.IsPaneOpen = false;

            var frame = this.MyTripsSplitView.Content as Frame;
            PageTitle.Text = ((Page)frame.Content).Name;
        }

        private void SelectControl(SplitViewButtonContent control)
        {
            if (selectedControl != null)
            {
                selectedControl.SetSelected(false);
            }
            control.SetSelected(true);
            selectedControl = control;
        }
    }
}
