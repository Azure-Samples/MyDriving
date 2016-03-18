using System;
using System.Linq;
using Xamarin.UITest;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace MyDriving.UITests
{
	public class CurrentTripPage : BasePage
	{
        readonly Query RecordingButton;
        readonly string UseSimulatorButton = "Use Simulator";
        readonly Query TripTitleField;
        readonly Query SaveTripButton;

		public CurrentTripPage ()
            : base("Current Trip", "Current Trip")
		{
            if (OnAndroid)
            {
                RecordingButton = x => x.Id("fab");
                TripTitleField = x => x.Class("EditText");
                SaveTripButton = x => x.Id("button1");
            }
            if (OniOS)
            { 
                RecordingButton = x => x.Class("UIButton");
                SaveTripButton = x => x.Marked("OK");
                TripTitleField = x => x.Class("UITextField");
            }
		}

		public CurrentTripPage StartRecordingTrip ()
		{
            app.Tap (RecordingButton);

            System.Threading.Thread.Sleep(2500);

            if (!app.Query(UseSimulatorButton).Any())
            {
                app.Tap(RecordingButton);
            }

            app.Tap(UseSimulatorButton);

            app.Screenshot ("Started recording trip");

			return this;
		}

		public CurrentTripPage StopRecordingTrip ()
		{
			System.Threading.Thread.Sleep (2500);
            app.Tap (RecordingButton);

			return this;
		}

		public CurrentTripPage SaveTrip (string title)
		{
            app.ClearText (TripTitleField);
            app.EnterText (TripTitleField, title);
			app.DismissKeyboard ();

			app.Screenshot ("Trip title entered");

            app.Tap(SaveTripButton);
            app.Screenshot("Trip Saved!");

            return this;
		}
	}
}