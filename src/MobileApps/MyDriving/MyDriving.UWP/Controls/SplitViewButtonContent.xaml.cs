using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MyDriving.UWP.Controls
{
    public sealed partial class SplitViewButtonContent : UserControl
    {
        public BitmapImage selectedImageSource;
        public BitmapImage defaultImageSource;
        public string labelText;

        SolidColorBrush selectedTextColor = new SolidColorBrush(Color.FromArgb(0xFF, 0x1b, 0xa0, 0xe1));
        SolidColorBrush defaultTextColor = new SolidColorBrush(Color.FromArgb(0xFF, 0xad, 0xac, 0xac));
        public SplitViewButtonContent()
        {
            this.InitializeComponent();
        }

        public void SetSelected(bool selected)
        {
            if (selected)
            {
                Image.Source = selectedImageSource;
                Label.Foreground = selectedTextColor;
            }
            else
            {
                Image.Source = defaultImageSource;
                Label.Foreground = defaultTextColor;
            }
        }


    }
}
