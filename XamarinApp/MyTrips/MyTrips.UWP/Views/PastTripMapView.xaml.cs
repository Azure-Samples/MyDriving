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
using MyTrips.DataObjects;
using MyTrips.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyTrips.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PastTripMapView : Page
    {


        public PastTripMapView()
        {
            this.InitializeComponent();
        }

        PastTripsDetailViewModel ViewModel;
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var trip = e.Parameter as Trip;
            base.OnNavigatedTo(e);

            ViewModel = new PastTripsDetailViewModel(trip);
            await ViewModel.ExecuteLoadTripCommandAsync(trip.Id);
            
        }
    }
}
