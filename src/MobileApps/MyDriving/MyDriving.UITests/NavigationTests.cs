using NUnit.Framework;
using Xamarin.UITest;

namespace MyDriving.UITests
{
	[TestFixture (Platform.Android)]
	[TestFixture (Platform.iOS)]
	public class NavigationTests
	{
		IApp app;
		Platform platform;

		[SetUp]
		public void BeforeEachTest()
		{
			app = AppInitializer.StartApp(platform);
		}

		[Test]
		public void AppLaunches()
		{
			app.Screenshot("First screen.");
		}

		[Test]
		public void NavigateToPastTrips()
		{
			app.Screenshot("First screen.");
			app.Tap("Skip Auth");
			app.Screenshot("Past Trips");
		}

		[Test]
		public void NavigateToPastTripsDetail()
		{
			app.Screenshot("First screen.");
			app.Tap("Skip Auth");
			app.Screenshot("Past Trips");
		}

		[Test]
		public void NavigateToCurrentTrip()
		{
			app.Screenshot("First screen.");
			app.Tap("Skip Auth");
			app.Screenshot("Past Trips");
		}

		[Test]
		public void NavigateToProfile()
		{
			app.Screenshot("First screen.");
			app.Tap("Skip Auth");
			app.Screenshot("Past Trips");
		}

		[Test]
		public void NavigateToSettings()
		{
			app.Screenshot("First screen.");
			app.Tap("Skip Auth");
			app.Screenshot("Past Trips");
		}

		[Test]
		public void NavigateToSettingsDetail()
		{
			app.Screenshot("First screen.");
			app.Tap("Skip Auth");
			app.Screenshot("Past Trips");
		}
	}
}