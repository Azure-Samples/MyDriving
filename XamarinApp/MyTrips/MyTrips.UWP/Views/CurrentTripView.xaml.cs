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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyTrips.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CurrentTripView : Page
    {
        CurrentTripViewModel viewModel;

        public CurrentTripView()
        {
            this.InitializeComponent();
            this.viewModel = new CurrentTripViewModel();

            this.startRecordBtn.Click += StartRecordBtn_Click;
            this.stopRecordBtn.Click += StopRecordBtn_Click;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel.StartTrackingTripCommand.Execute(null);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //Ideally, we should stop tracking only if we aren't recording
            viewModel.StopTrackingTripCommand.Execute(null);
        }

        private async void StartRecordBtn_Click(object sender, RoutedEventArgs e)
        {
            await this.viewModel.StartRecordingTripAsync();
        }

        private async void StopRecordBtn_Click(object sender, RoutedEventArgs e)
        {
            await this.viewModel.StopRecordingTripAsync();
        }
    }
}
