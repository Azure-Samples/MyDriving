using System;
using Xamarin.UITest;
using NUnit.Framework;

namespace MyTrips.UITests
{
	[TestFixture (Platform.iOS)]
	public class CurrentTripTests : AbstractSetup
	{
		public CurrentTripTests (Platform platform) : base (platform)
		{
		}

		[Test]
		public void RecordTripTest ()
		{
			new CurrentTripPage ()
				.NavigateToCurrentTripPage ()
				.StartRecordingTrip ()
				.StopRecordingTrip ()
				.EnterTripName ()
				.DismissTripSummary ();
		}
	}
}

