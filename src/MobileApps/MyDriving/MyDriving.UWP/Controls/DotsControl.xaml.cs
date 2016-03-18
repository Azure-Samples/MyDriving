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
using Windows.UI;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MyDriving.UWP.Controls
{
    public sealed partial class DotsControl : UserControl
    {
        SolidColorBrush selectedColorBrush = new SolidColorBrush(Colors.White);
        SolidColorBrush unselectedColorBrush = new SolidColorBrush(Colors.Gray);
        public DotsControl()
        {
            this.InitializeComponent();
        }

        public void SelectCircle(int position)
        {
            switch(position)
            {
                case 1:
                    Circle5.Fill = unselectedColorBrush;
                    Circle1.Fill = selectedColorBrush;
                    break;
                case 2:
                    Circle1.Fill = unselectedColorBrush;
                    Circle2.Fill = selectedColorBrush;
                    break;
                case 3:
                    Circle2.Fill = unselectedColorBrush;
                    Circle3.Fill = selectedColorBrush;
                    break;
                case 4:
                    Circle3.Fill = unselectedColorBrush;
                    Circle4.Fill = selectedColorBrush;
                    break;
                case 5:
                    Circle4.Fill = unselectedColorBrush;
                    Circle5.Fill = selectedColorBrush;
                    break;
                default:
                    UnselectAll();
                    break;
            }
        }

        public void UnselectAll()
        {
            Circle1.Fill = unselectedColorBrush;
            Circle2.Fill = selectedColorBrush;
            Circle3.Fill = unselectedColorBrush;
            Circle4.Fill = selectedColorBrush;
            Circle5.Fill = selectedColorBrush;
        }
    }
}
