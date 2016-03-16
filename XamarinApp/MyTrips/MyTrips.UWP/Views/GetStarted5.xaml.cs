using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyTrips.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GetStarted5 : Page
    {
        private double StartX;
        private double EndX;
        public GetStarted5()
        {
            this.InitializeComponent();
            Dots.SelectCircle(5);

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
            if (EndX > StartX) //back
                this.Frame.Navigate(typeof(GetStarted4));
            e.Handled = true;
        }

        private void GoNext(object sender, RoutedEventArgs e)
        {
            Settings.Current.FirstRun = false;
            Window.Current.Content = new SplitViewShell(this.Frame);
            this.Frame.Navigate(typeof(LoginView));
        }

    }
}

