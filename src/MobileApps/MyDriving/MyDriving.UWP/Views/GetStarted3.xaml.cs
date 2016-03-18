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

namespace MyDriving.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GetStarted3 : Page
    {
        private double StartX;
        private double EndX;
        public GetStarted3()
        {
            this.InitializeComponent();
            Dots.SelectCircle(3);

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
            if (EndX < StartX)  //forward
                this.Frame.Navigate(typeof(GetStarted4));
            else if (EndX > StartX) //back
                this.Frame.Navigate(typeof(GetStarted2));
            e.Handled = true;
        }
    }
}
