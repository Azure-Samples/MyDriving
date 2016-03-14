using System;
using Xamarin.UITest;

namespace MyTrips.UITests
{
	public class CurrentTripPage : BasePage
	{
		public CurrentTripPage ()
		{
			app.Screenshot ("Current Trip");
		}

		public CurrentTripPage NavigateToCurrentTripPage ()
		{
			app.Tap ("Current Trip");

			return this;
		}

		public CurrentTripPage StartRecordingTrip ()
		{
			app.Tap (c => c.Class ("UIButton"));
			app.Screenshot ("Started recording trip");

			return this;
		}

		public CurrentTripPage StopRecordingTrip ()
		{
			System.Threading.Thread.Sleep (2500);
			app.Tap (c => c.Class ("UIButton"));

			return this;
		}

		public CurrentTripPage EnterTripName ()
		{
			app.EnterText (c => c.Class ("UITextField"), "Trip Name");
			app.DismissKeyboard ();
			app.Screenshot ("Trip summary");

			return this;
		}

		public CurrentTripPage DismissTripSummary ()
		{
			app.Tap ("Done");

			return this;
		}
	}
}