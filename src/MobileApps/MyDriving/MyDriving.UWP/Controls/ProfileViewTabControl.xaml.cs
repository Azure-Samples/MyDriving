using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MyDriving.UWP.Controls
{
    public sealed partial class ProfileViewTabControl : UserControl
    {
        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public ProfileViewTabControl()
        {
            this.InitializeComponent();
        }
    }
}
