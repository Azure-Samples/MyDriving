// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using UIKit;

namespace MyDriving.iOS
{
    public partial class SettingDetailTableViewCell : UITableViewCell
    {
        public SettingDetailTableViewCell(IntPtr handle) : base(handle)
        {
        }

        public string Name
        {
            get { return settingNameLabel.Text; }
            set { settingNameLabel.Text = value; }
        }

        public bool Checked
        {
            get { return Accessory == UITableViewCellAccessory.Checkmark; }
            set { Accessory = value ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None; }
        }
    }
}