using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

using MyTrips.Model;
using MyTrips.ViewModel;

namespace MyTrips.iOS
{
	partial class SettingsViewController : UIViewController
	{
		SettingsViewModel ViewModel { get; set; }

		public SettingsViewController (IntPtr handle) : base (handle)
		{
			
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Wire up view model
			ViewModel = new SettingsViewModel();

			// Wire up table source
			settingsTableView.Source = new SettingsDataSource(ViewModel);
		}
	}

	// TODO: Add custom header view
	// TODO: Add detail view
	// TODO: Alter shared logic
	public class SettingsDataSource : UITableViewSource
	{
		Dictionary<string, List<Setting>> data;
		SettingsViewModel viewModel;

		string[] keys;

		public SettingsDataSource(SettingsViewModel viewModel)
		{
			this.viewModel = viewModel;
			data = viewModel.SettingsData;

			keys = new string[data.Count];
			int i = 0;
			foreach (var grouping in data) {
				keys[i] = grouping.Key;
				i++;
			}
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return keys.Length;
		}

		public override string TitleForHeader(UITableView tableView, nint section)
		{
			return keys[section];
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return data[keys[section]].Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell("SETTING_VALUE_CELL") as SettingTableViewCell;

			var setting = data[keys[indexPath.Section]][indexPath.Row];
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