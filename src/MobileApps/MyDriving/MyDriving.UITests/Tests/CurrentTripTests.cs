// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Xamarin.UITest;
using NUnit.Framework;

namespace MyDriving.UITests
{
    [TestFixture(Platform.Android)]
    public class CurrentTripTests : AbstractSetup
    {
        public CurrentTripTests(Platform platform) : base(platform)
        {
        }

        [Test]
        public void RecordTripTest()
        {
            new CurrentTripPage()
                .NavigateToCurrentTripPage()
                .StartRecordingTrip()
                .StopRecordingTrip();
            //.DismissTripSummary ()
            //.EnterTripName();
        }
    }
}