// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MyDriving.UWP.Controls
{
    public sealed partial class SplitViewButtonContent
    {
        readonly SolidColorBrush defaultTextColor = new SolidColorBrush(Colors.White);

        readonly SolidColorBrush selectedTextColor = new SolidColorBrush(Color.FromArgb(0xFF, 0x1b, 0xa0, 0xe1));
        public BitmapImage DefaultImageSource;
        public string LabelText;
        public BitmapImage SelectedImageSource;

        public SplitViewButtonContent()
        {
            InitializeComponent();
        }

        public void SetSelected(bool selected)
        {
            if (selected)
            {
                Image.Source = SelectedImageSource;
                Label.Foreground = selectedTextColor;
            }
            else
            {
                Image.Source = DefaultImageSource;
                Label.Foreground = defaultTextColor;
            }
        }
    }
}