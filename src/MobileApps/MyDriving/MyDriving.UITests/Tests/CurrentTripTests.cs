// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

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
        public void RecordTripTest()
        {
            new CurrentTripPage()
                .NavigateTo("Current Trip");

            new CurrentTripPage()
                .StartRecordingTrip()
                .StopRecordingTrip()
                .SaveTrip("Test Cloud Test Drive");

            new TripSummaryPage()
                .AssertOnPage();
		}
	}
}