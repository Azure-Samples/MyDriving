using System;
using System.Threading;
using Xamarin.UITest;
using NUnit.Framework;
using System.Linq;
using Xamarin.UITest.Android;
using Xamarin.UITest.iOS;

namespace MyTrips.UITests
{
	[TestFixture(Platform.Android)]
	[TestFixture(Platform.iOS)]
	public abstract class AbstractSetup
	{
		protected IApp app;
		protected Platform platform;
		protected bool OnAndroid;
		protected bool OniOS;

		public AbstractSetup(Platform platform)
		{
			this.platform = platform;
		}

		[SetUp]
		public virtual void BeforeEachTest()
		{
			app = AppInitializer.StartApp(platform);
			OnAndroid = app.GetType() == typeof(AndroidApp);
			OniOS = app.GetType() == typeof(iOSApp);

			if (app.Query("LoginPageIdentifier").Any())
			{
				new LoginPage ()
					.SkipAuthentication ();
				
				Thread.Sleep(2000);
			}

			if (app.Query ("Allow").Any ())
				app.Tap ("Allow");
		}

		public void ClearKeychain ()
		{
			app.TestServer.Post("/keychain", new object());
			app = ConfigureApp.iOS.StartApp();
		}
	}
}