using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using System.Globalization;

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

			btnLogout.TouchUpInside += async delegate {
				await ViewModel.ExecuteLogoutCommandAsync();
				Acr.UserDialogs.UserDialogs.Instance.ShowSuccess("Successfully logged out");
			};

			btnLogout.Layer.CornerRadius = 4;
			btnLogout.Layer.MasksToBounds = true;

		}


		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "settingsDetailSegue")
			{
				var controller = segue.DestinationViewController as SettingsDetailViewController;
				var indexPath = settingsTableView.IndexPathForCell(sender as UITableViewCell);

				// TODO: Set this to a real key
				var setting = ViewModel.SettingsData["Units"][0];

				settingsTableView.DeselectRow(settingsTableView.IndexPathForSelectedRow, true);

				controller.Setting = setting;
			}
		}
	}

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

		public override void WillDisplayHeaderView(UITableView tableView, UIView headerView, nint section)
		{
			var header = headerView as UITableViewHeaderFooterView;
			header.TextLabel.TextColor = "5C5C5C".ToUIColor();
			header.TextLabel.Font = UIFont.FromName("AvenirNext-Medium", 16);
			header.TextLabel.Text = TitleForHeader(tableView, section);
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