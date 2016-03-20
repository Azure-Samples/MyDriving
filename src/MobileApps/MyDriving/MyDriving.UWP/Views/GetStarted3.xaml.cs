// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyDriving.UWP.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GetStarted3
    {
        private double endX;
        private double startX;

        public GetStarted3()
        {
            InitializeComponent();
            Dots.SelectCircle(3);

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
            if (endX < startX) //forward
                Frame.Navigate(typeof (GetStarted4));
            else if (endX > startX) //back
                Frame.Navigate(typeof (GetStarted2));
            e.Handled = true;
        }
    }
}