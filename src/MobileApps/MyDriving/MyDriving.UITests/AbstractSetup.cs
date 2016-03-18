// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Threading;
using Xamarin.UITest;
using NUnit.Framework;
using System.Linq;
using Xamarin.UITest.Android;
using Xamarin.UITest.iOS;

namespace MyDriving.UITests
{
    [TestFixture(Platform.iOS)]
    public abstract class AbstractSetup
    {
        [SetUp]
        public virtual void BeforeEachTest()
        {
            App = AppInitializer.StartApp(Platform);
            OnAndroid = App.GetType() == typeof (AndroidApp);
            OniOS = App.GetType() == typeof (iOSApp);

            if (App.Query("Login with Facebook").Any())
            {
                new LoginPage()
                    .SkipAuthentication();

                Thread.Sleep(2000);
            }

            if (OniOS)
            {
                if (App.Query("Allow").Any())
                    App.Tap("Allow");
            }
        }

        protected IApp App;
        protected Platform Platform;
        protected bool OnAndroid;
        protected bool OniOS;

        public AbstractSetup(Platform platform)
        {
            Platform = platform;
        }

        public void ClearKeychain()
        {
            if (OnAndroid)
                return;

            if (!App.Query("LoginWithFacebook").Any())
            {
                App.TestServer.Post("/keychain", new object());
                App = ConfigureApp.iOS.StartApp();
            }
        }
    }
}