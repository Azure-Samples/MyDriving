using MyTrips.DataObjects;
using MyTrips.ViewModel;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MyTrips.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PastTripsMenuView : Page
    {
        public PastTripsMenuView()
        {
            this.InitializeComponent();
            this.ViewModel = new PastTripsViewModel();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await this.ViewModel.ExecuteLoadPastTripsCommandAsync();
        }

        public PastTripsViewModel ViewModel { get; set; }

        private async void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            var trip = (Trip)e.AddedItems[0];
            this.Frame.Navigate(typeof(PastTripMapView),trip); // PastTripMapView does not exist
        }

    }
}
