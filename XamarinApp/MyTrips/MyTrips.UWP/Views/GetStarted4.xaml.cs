using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyTrips.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GetStarted4 : Page
    {
        private double StartX;
        private double EndX;
        public GetStarted4()
        {
            this.InitializeComponent();
            Dots.SelectCircle(4);

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
                this.Frame.Navigate(typeof(GetStarted5));
            else if (EndX > StartX) //back
                this.Frame.Navigate(typeof(GetStarted3));
            e.Handled = true;
        }
    }
}
