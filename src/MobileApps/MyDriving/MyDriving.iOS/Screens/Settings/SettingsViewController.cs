// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using MyDriving.Model;
using MyDriving.ViewModel;

namespace MyDriving.iOS
{
    partial class SettingsViewController : UIViewController
    {
        string[] _keys;

        public SettingsViewController(IntPtr handle) : base(handle)
        {
        }

        SettingsViewModel ViewModel { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Wire up view model
            ViewModel = new SettingsViewModel();

            _keys = new string[ViewModel.SettingsData.Count];
            int i = 0;
            foreach (var grouping in ViewModel.SettingsData)
            {
                _keys[i] = grouping.Key;
                i++;
            }

            // Wire up table source
            settingsTableView.Source = new SettingsDataSource(ViewModel, _keys);

            btnLogout.Hidden = true;

            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("RefreshSettingsTable"),
                HandleReloadTableNotification);
        }

        public override bool ShouldPerformSegue(string segueIdentifier, NSObject sender)
        {
            var cell = (SettingTableViewCell) sender;
            if (cell.Accessory == UITableViewCellAccessory.None)
                return false;
            else
                return true;
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "settingsDetailSegue")
            {
                var controller = segue.DestinationViewController as SettingsDetailViewController;
                var cell = settingsTableView.IndexPathForCell(sender as UITableViewCell);
                var row = cell.Row;
                var section = cell.Section;

                var setting = ViewModel.SettingsData[_keys[section]][row];

                settingsTableView.DeselectRow(settingsTableView.IndexPathForSelectedRow, true);

                controller.Setting = setting;
                controller.ViewModel = ViewModel;
                controller.SettingKey = _keys[cell.Section];
            }
        }

        void HandleReloadTableNotification(NSNotification obj)
        {
            InvokeOnMainThread(delegate { settingsTableView.ReloadData(); });
        }
    }

    public class SettingsDataSource : UITableViewSource
    {
        readonly Dictionary<string, List<Setting>> _data;

        readonly string[] _keys;
        readonly SettingsViewModel _viewModel;

        public SettingsDataSource(SettingsViewModel viewModel, string[] keys)
        {
            _viewModel = viewModel;
            _data = viewModel.SettingsData;

            _keys = keys;
        }

        public override void WillDisplayHeaderView(UITableView tableView, UIView headerView, nint section)
        {
            var header = headerView as UITableViewHeaderFooterView;
            header.TextLabel.TextColor = "5C5C5C".ToUIColor();
            header.TextLabel.Font = UIFont.FromName("AvenirNext-Medium", 16);
            header.TextLabel.Text = TitleForHeader(tableView, section);
        }

        public override async void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var setting = _data[_keys[indexPath.Section]][indexPath.Row];
            if (setting.IsButton)
            {
                if (setting.ButtonUrl != "Permissions")
                {
                    await _viewModel.ExecuteOpenBrowserCommandAsync(setting.ButtonUrl);
                }
                else
                {
                    var url = NSUrl.FromString(UIApplication.OpenSettingsUrlString);
                    UIApplication.SharedApplication.OpenUrl(url);
                }
            }
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return _keys.Length;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return _keys[section];
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return _data[_keys[section]].Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("SETTING_VALUE_CELL") as SettingTableViewCell;

            var setting = _data[_keys[indexPath.Section]][indexPath.Row];
            if (setting.IsButton)
            {
                cell.Accessory = UITableViewCellAccessory.None;
                cell.Value = "";
            }
            else
            {
                cell.Value = !String.IsNullOrEmpty(setting.Value) ? cell.Value = setting.Value : cell.Value = "";
            }

            cell.Name = setting.Name;

            return cell;
        }
    }
}