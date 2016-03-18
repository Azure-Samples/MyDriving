// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyDriving.UWP.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GetStarted1 : Page
    {
        private double EndX;
        private double StartX;

        public GetStarted1()
        {
            InitializeComponent();
            Dots.SelectCircle(1);

            ManipulationMode = ManipulationModes.TranslateX;
            ManipulationStarted += Manipulation_Started;
            ManipulationCompleted += Manipulation_Completed;
        }


        void Manipulation_Started(object sender, ManipulationStartedRoutedEventArgs e)
        {
            StartX = e.Position.X;
            e.Handled = true;
        }

        void Manipulation_Completed(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            EndX = e.Position.X;
            if (EndX < StartX)
                Frame.Navigate(typeof (GetStarted2));
            e.Handled = true;
        }
    }
}