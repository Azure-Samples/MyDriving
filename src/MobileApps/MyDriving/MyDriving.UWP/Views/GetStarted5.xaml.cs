// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using MyDriving.Utils;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyDriving.UWP.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GetStarted5
    {
        private double endX;
        private double startX;

        public GetStarted5()
        {
            InitializeComponent();
            Dots.SelectCircle(5);

            ManipulationMode = ManipulationModes.TranslateX;
            ManipulationStarted += Manipulation_Started;
            ManipulationCompleted += Manipulation_Completed;
        }


        void Manipulation_Started(object sender, ManipulationStartedRoutedEventArgs e)
        {
            startX = e.Position.X;
            e.Handled = true;
        }

        void Manipulation_Completed(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            endX = e.Position.X;
            if (endX > startX) //back
                Frame.Navigate(typeof (GetStarted4));
            e.Handled = true;
        }

        private void GoNext(object sender, RoutedEventArgs e)
        {
            Settings.Current.FirstRun = false;
            Frame.Navigate(typeof (LoginView));
        }
    }
}