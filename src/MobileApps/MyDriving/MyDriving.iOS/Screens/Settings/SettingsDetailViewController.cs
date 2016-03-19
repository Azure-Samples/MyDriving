// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Foundation;
using UIKit;
using MyDriving.Model;
using MyDriving.ViewModel;

namespace MyDriving.iOS
{
    public partial class SettingsDetailViewController : UIViewController
    {
        public SettingsViewModel ViewModel;

        public SettingsDetailViewController(IntPtr handle) : base(handle)
        {
        }

        public string SettingKey { get; set; }
        public Setting Setting { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.Title = Setting.Name;
            settingsDetailTableView.Source = new SettingsDetailTableViewSource(SettingKey, Setting);

            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("SettingTextFieldChanged"),
                HandleSettingChangedNotification);
        }

        void HandleSettingChangedNotification(NSNotification obj)
        {
            var dict = (NSDictionary) obj.Object;
            var row = dict.ObjectForKey(new NSString("Row")) as NSString;
            var section = dict.ObjectForKey(new NSString("Section")) as NSString;
            var value = dict.ObjectForKey(new NSString("Value")) as NSString;

            ViewModel.SettingsData[section.ToString()][Int32.Parse(row.ToString())].Value = value.ToString();
        }
    }

    public class SettingsDetailTableViewSource : UITableViewSource
    {
        readonly string key;
        readonly Setting setting;

        public SettingsDetailTableViewSource(string key, Setting setting)
        {
            this.setting = setting;
            this.key = key;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return setting.Name;
        }

        public override void WillDisplayHeaderView(UITableView tableView, UIView headerView, nint section)
        {
            var header = headerView as UITableViewHeaderFooterView;
            header.TextLabel.TextColor = "5C5C5C".ToUIColor();
            header.TextLabel.Font = UIFont.FromName("AvenirNext-Medium", 16);
            header.TextLabel.Text = TitleForHeader(tableView, section);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (!setting.IsButton)
            {
                setting.Value = setting.PossibleValues[indexPath.Row];

                var cells = tableView.VisibleCells;
                int i = 0;
                foreach (var cell in cells)
                {
                    var value = setting.PossibleValues[i];
                    cell.Accessory = setting.Value != value
                        ? UITableViewCellAccessory.None
                        : UITableViewCellAccessory.Checkmark;

                    i++;
                }

                NSNotificationCenter.DefaultCenter.PostNotificationName("RefreshTripUnits", null);
                NSNotificationCenter.DefaultCenter.PostNotificationName("RefreshSettingsTable", null);
            }
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (!setting.IsTextField)
                return setting.PossibleValues.Count;
            else
                return 1;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (!setting.IsTextField)
            {
                var cell = tableView.DequeueReusableCell("SETTING__DETAIL_VALUE_CELL") as SettingDetailTableViewCell;

                cell.Name = setting.PossibleValues[indexPath.Row];
                cell.Accessory = UITableViewCellAccessory.None;

                if (cell.Name == setting.Value)
                    cell.Accessory = UITableViewCellAccessory.Checkmark;

                return cell;
            }
            else
            {
                var cell =
                    tableView.DequeueReusableCell("SETTING_DETAIL_TEXTFIELD_CELL") as
                        SettingDetailTextFieldTableViewCell;

                cell.Row = indexPath.Row.ToString();
                cell.Section = key;

                cell.Value = setting.Value ?? string.Empty;

                return cell;
            }
        }
    }
}