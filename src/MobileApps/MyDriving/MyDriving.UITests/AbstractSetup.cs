using System;
using System.Threading;
using Xamarin.UITest;
using NUnit.Framework;
using System.Linq;
using Xamarin.UITest.Android;
using Xamarin.UITest.iOS;

namespace MyDriving.UITests
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

			if (app.Query("Login with Facebook").Any())
			{
				new LoginPage ()
					.SkipAuthentication ();
				
				Thread.Sleep(2000);
			}

            if (OniOS)
            {
                if (app.Query("Allow").Any())
                    app.Tap ("Allow");
            }
		}

		public void ClearKeychain ()
		{
            if (OnAndroid)
            {
                app = ConfigureApp.Android.ApkFile(AppInitializer.apkPath).StartApp();
                return;
            }

            else
            {
                if (!app.Query("LoginWithFacebook").Any())
                {
                    app.TestServer.Post("/keychain", new object());
                    app = ConfigureApp.iOS.InstalledApp("com.microsoft.mydriving").StartApp();
    			}
            }
		}
	}
}