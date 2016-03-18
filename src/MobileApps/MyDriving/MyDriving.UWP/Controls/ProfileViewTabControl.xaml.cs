// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MyDriving.UWP.Controls
{
    public sealed partial class ProfileViewTabControl : UserControl
    {
        public ProfileViewTabControl()
        {
            InitializeComponent();
        }

        public string Title1 { get; set; }
        public string Title2 { get; set; }
    }
}