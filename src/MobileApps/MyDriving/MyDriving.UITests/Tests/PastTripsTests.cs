// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Xamarin.UITest;
using NUnit.Framework;

namespace MyDriving.UITests
{
    [TestFixture(Platform.Android)]
    public class PastTripsTests : AbstractSetup
    {
        public PastTripsTests(Platform platform) : base(platform)
        {
        }

        [Test]
        public void ClickTripSliderEndpointsTest()
        {
            new PastTripsPage()
                .NavigateToPastTripsPage()
                .NavigateToPastTripsDetail()
                .ClickTripSliderEndpoints();
        }

        [Test]
        public void MoveTripSliderTest()
        {
            new PastTripsPage()
                .NavigateToPastTripsPage()
                .NavigateToPastTripsDetail()
                .MoveTripSlider();
        }

        [Test]
        public void NavigateToDetailsTest()
        {
            new PastTripsPage()
                .NavigateToPastTripsPage()
                .NavigateToPastTripsDetail();
        }

        [Test]
        public void PullToRefreshTest()
        {
            new PastTripsPage()
                .NavigateToPastTripsPage()
                .PullToRefresh();
        }
    }
}