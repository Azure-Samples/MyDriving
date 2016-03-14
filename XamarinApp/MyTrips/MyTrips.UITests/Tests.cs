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

//		[Test]
//		public void Repl()
//		{
//			app.Repl ();
//		}
//
//        [Test]
//        public void AppLaunchesSuccessfully()
//        {
//            app.Screenshot("First screen.");
//        }

//		[Test]
//		public void SkipAuthentication()
//		{
//			new LoginPage ()
//				.SkipAuthentication ();
//
//			app.Screenshot ("Authentication Skipped");
//		}

		[Test]
		public void LoginWithFacebook()
		{
			new LoginPage ()
				.LoginWithFacebook ();

			app.Screenshot ("Facebook Authentication Succeeded");
		}
    }
}

