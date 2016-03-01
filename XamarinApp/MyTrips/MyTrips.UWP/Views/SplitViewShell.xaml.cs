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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyTrips.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SplitViewShell : Page
    {
        public SplitViewShell(Frame frame)
        {
            this.InitializeComponent();
            this.MyTripsSplitView.Content = frame;
            frame.Navigated += Frame_Navigated;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MyTripsSplitView.IsPaneOpen = !MyTripsSplitView.IsPaneOpen;
        }

        private void RecommendedRoutesButton_Click(object sender, RoutedEventArgs e)
        {
            MyTripsSplitView.IsPaneOpen = false;
            ((Frame)this.MyTripsSplitView.Content).Navigate(typeof(RecommendedRoutes));
        }

        private void TripsButton_Click(object sender, RoutedEventArgs e)
        {
            MyTripsSplitView.IsPaneOpen = false;
            ((Frame)this.MyTripsSplitView.Content).Navigate(typeof(PastTripsMenuView));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MyTripsSplitView.IsPaneOpen = false;
            ((Frame)this.MyTripsSplitView.Content).Navigate(typeof(SettingsView));
        }

        private void NewTripButton_Click(object sender, RoutedEventArgs e)
        {
            MyTripsSplitView.IsPaneOpen = false;
            ((Frame)this.MyTripsSplitView.Content).Navigate(typeof(CurrentTripView));
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            MyTripsSplitView.IsPaneOpen = false;
            
            var frame = this.MyTripsSplitView.Content as Frame;
            PageTitle.Text = ((Page)frame.Content).Name;
        }
    }
}
