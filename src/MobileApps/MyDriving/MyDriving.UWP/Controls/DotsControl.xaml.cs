// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace MyDriving.UWP.Controls
{
    public sealed partial class DotsControl
    {
        readonly SolidColorBrush selectedColorBrush = new SolidColorBrush(Colors.White);
        readonly SolidColorBrush unselectedColorBrush = new SolidColorBrush(Colors.Gray);

        public DotsControl()
        {
            InitializeComponent();
        }

        public void SelectCircle(int position)
        {
            switch (position)
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