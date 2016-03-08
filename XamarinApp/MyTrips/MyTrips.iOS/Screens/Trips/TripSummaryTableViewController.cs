using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace MyTrips.iOS
{
    public partial class TripSummaryTableViewController : UIViewController
    {
		public ViewModel.CurrentTripViewModel ViewModel { get; set; }

		public TripSummaryTableViewController(IntPtr handle) : base(handle) { }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var data = new List<DrivingStatistic>
			{
				new DrivingStatistic { Name = "Total Distance", Value = $"{ViewModel.Distance} {ViewModel.DistanceUnits.ToLower()}" },
				new DrivingStatistic { Name = "Total Duration", Value = ViewModel.ElapsedTime },
				new DrivingStatistic { Name = "Total Fuel Consumption", Value = ViewModel.FuelConsumptionUnits},
				new DrivingStatistic { Name = "Average Speed", Value = "31 MPH"},
				new DrivingStatistic { Name = "Hard Breaks", Value = "21"},
				new DrivingStatistic { Name = "Tips Received", Value = "14"},
			};

			tripSummaryTableView.Source = new TripSummaryTableViewSource(data);

			tripNameTextField.ReturnKeyType = UIReturnKeyType.Done;
			tripNameTextField.Delegate = new TextViewDelegate();
		}

		async partial void DoneButton_TouchUpInside(UIButton sender)
		{
			await ViewModel.SaveRecordingTripAsync(tripNameTextField.Text);
			NSNotificationCenter.DefaultCenter.PostNotificationName("RefreshPastTripsTable", null);

			DismissViewController(true, null);
		}

		public class TextViewDelegate : UITextFieldDelegate
		{
			public override bool ShouldReturn(UITextField textField)
			{
				textField.ResignFirstResponder();

				return true;
			}
		}

		public class TripSummaryTableViewSource : UITableViewSource
		{
			const string TRIP_SUMMARY_CELL_IDENTIFIER = "TRIP_SUMMARY_CELL_IDENTIFIER";

			List<DrivingStatistic> data;

			public TripSummaryTableViewSource(List<DrivingStatistic> data)
			{
				this.data = data;
			}

			public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
			{
				return 60f;
			}

			public override nint RowsInSection(UITableView tableview, nint section)
			{
				return data.Count;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(TRIP_SUMMARY_CELL_IDENTIFIER) as TripSummaryCell;

				if (cell == null)
					cell = new TripSummaryCell(new NSString(TRIP_SUMMARY_CELL_IDENTIFIER));

				cell.Name = data[indexPath.Row].Name;
				cell.Value = data[indexPath.Row].Value;

				return cell;
			}
		}
	}
}