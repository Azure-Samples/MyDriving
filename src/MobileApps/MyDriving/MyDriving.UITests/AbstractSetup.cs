// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
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
            app = AppInitializer.StartApp(platform);
            OnAndroid = app.GetType() == typeof (AndroidApp);
            OniOS = app.GetType() == typeof (iOSApp);

            if (app.Query("Login with Facebook").Any())
            {
                new LoginPage()
                    .SkipAuthentication();

                Thread.Sleep(2000);
            }

            if (OniOS)
            {
                if (app.Query("Allow").Any())
                    app.Tap("Allow");
            }
        }

        protected IApp app;
        protected Platform platform;
        protected bool OnAndroid;
        protected bool OniOS;

        public AbstractSetup(Platform platform)
        {
            this.platform = platform;
        }

        public void ClearKeychain()
        {
            if (OnAndroid)
                return;

            if (!app.Query("LoginWithFacebook").Any())
            {
                app.TestServer.Post("/keychain", new object());
                app = ConfigureApp.iOS.StartApp();
            }
        }
    }
}