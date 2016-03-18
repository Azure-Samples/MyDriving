using System;
using Xamarin.UITest;
using NUnit.Framework;

namespace MyDriving.UITests
{
	public class CurrentTripTests : AbstractSetup
	{
		public CurrentTripTests (Platform platform) : base (platform)
		{
		}

		[Test]
		public void RecordTripTest ()
		{
            new CurrentTripPage()
                .NavigateToCurrentTripPage()
                .StartRecordingTrip()
                .StopRecordingTrip()
                .SaveTrip("Test Cloud Test Drive");

            //TODO: verify trip saved
		}
	}
}

