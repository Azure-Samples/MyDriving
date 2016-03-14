using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace MyTrips.UITests
{
    [TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

		[SetUp]
		public void BeforeEachTest()
		{
			app = AppInitializer.StartApp(platform);
		}

		// If you would like to play around with the Xamarin.UITest REPL
		// uncomment out this method, and run this test with the NUnit test runner.

//		[Test]
//		public void Repl()
//		{
//			app.Repl ();
//		}
    }
}