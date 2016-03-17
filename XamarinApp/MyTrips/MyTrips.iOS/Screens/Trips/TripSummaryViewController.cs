using System;
using UIKit;

namespace MyTrips.iOS
{
    public partial class TripSummaryViewController : UIViewController
    {
		public ViewModel.CurrentTripViewModel ViewModel { get; set; }

		public TripSummaryViewController(IntPtr handle) : base(handle) { }

		public override void ViewDidLoad()
		{
			lblDateTime.Text = $"{DateTime.Now.ToString("M")}  {DateTime.Now.ToString("t")}";
			lblDistance.Text = $"{ViewModel.Distance} {ViewModel.DistanceUnits.ToLower()}";
			lblDuration.Text = ViewModel.ElapsedTime;
			lblFuelConsumed.Text = $"{ViewModel.FuelConsumption} {ViewModel.FuelConsumptionUnits.ToLower()}";

			lblDistance.Alpha = 0;
			lblDuration.Alpha = 0;
			lblTopSpeed.Alpha = 0;
			lblFuelConsumed.Alpha = 0;
		}

		public override async void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			lblDistance.FadeIn(0.4, 0.1f);
			lblDuration.FadeIn(0.4, 0.2f);
			lblTopSpeed.FadeIn(0.4, 0.3f);
			lblFuelConsumed.FadeIn(0.4, 0.4f);

			await ViewModel.SaveRecordingTripAsync();
		}

		async partial void BtnClose_TouchUpInside(UIButton sender)
		{
			await DismissViewControllerAsync(true);
		}
	}
}