// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MyDriving.UWP.Controls
{
    public sealed partial class DotsControl : UserControl
    {
        readonly SolidColorBrush _selectedColorBrush = new SolidColorBrush(Colors.White);
        readonly SolidColorBrush _unselectedColorBrush = new SolidColorBrush(Colors.Gray);

        public DotsControl()
        {
            InitializeComponent();
        }

        public void SelectCircle(int position)
        {
            switch (position)
            {
                case 1:
                    Circle5.Fill = _unselectedColorBrush;
                    Circle1.Fill = _selectedColorBrush;
                    break;
                case 2:
                    Circle1.Fill = _unselectedColorBrush;
                    Circle2.Fill = _selectedColorBrush;
                    break;
                case 3:
                    Circle2.Fill = _unselectedColorBrush;
                    Circle3.Fill = _selectedColorBrush;
                    break;
                case 4:
                    Circle3.Fill = _unselectedColorBrush;
                    Circle4.Fill = _selectedColorBrush;
                    break;
                case 5:
                    Circle4.Fill = _unselectedColorBrush;
                    Circle5.Fill = _selectedColorBrush;
                    break;
                default:
                    UnselectAll();
                    break;
            }
        }

        public void UnselectAll()
        {
            Circle1.Fill = _unselectedColorBrush;
            Circle2.Fill = _selectedColorBrush;
            Circle3.Fill = _unselectedColorBrush;
            Circle4.Fill = _selectedColorBrush;
            Circle5.Fill = _selectedColorBrush;
        }
    }
}