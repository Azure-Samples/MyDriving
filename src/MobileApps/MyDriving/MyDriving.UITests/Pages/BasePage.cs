﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using Xamarin.UITest.Android;
using Xamarin.UITest.iOS;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;
using System.Linq;

namespace MyDriving.UITests
{
    public class BasePage
    {
        protected readonly IApp App;
        protected readonly bool OnAndroid;
        protected readonly bool OniOS;

        protected Func<AppQuery, AppQuery> Trait;

        protected BasePage()
        {
            App = AppInitializer.App;

            OnAndroid = App.GetType() == typeof (AndroidApp);
            OniOS = App.GetType() == typeof (iOSApp);

            InitializeCommonQueries();
        }

        protected BasePage(Func<AppQuery, AppQuery> androidTrait, Func<AppQuery, AppQuery> iOSTrait)
            : this()
        {
            if (OnAndroid)
                Trait = androidTrait;
            if (OniOS)
                Trait = iOSTrait;

            AssertOnPage(TimeSpan.FromSeconds(30));

            App.Screenshot("On " + GetType().Name);
        }

        protected BasePage(string androidTrait, string iOSTrait)
            : this(x => x.Marked(androidTrait), x => x.Marked(iOSTrait))
        {
        }

        /// <summary>
        ///     Verifies that the trait is still present. Defaults to no wait.
        /// </summary>
        /// <param name="timeout">Time to wait before the assertion fails</param>
        protected void AssertOnPage(TimeSpan? timeout = default(TimeSpan?))
        {
            if (Trait == null)
                throw new NullReferenceException("Trait not set");

            var message = "Unable to verify on page: " + GetType().Name;

            if (timeout == null)
                Assert.IsNotEmpty(App.Query(Trait), message);
            else
                Assert.DoesNotThrow(() => App.WaitForElement(Trait, timeout: timeout), message);
        }

        /// <summary>
        ///     Verifies that the trait is no longer present. Defaults to a two second wait.
        /// </summary>
        /// <param name="timeout">Time to wait before the assertion fails</param>
        protected void WaitForPageToLeave(TimeSpan? timeout = default(TimeSpan?))
        {
            if (Trait == null)
                throw new NullReferenceException("Trait not set");

            timeout = timeout ?? TimeSpan.FromSeconds(2);
            var message = "Unable to verify *not* on page: " + GetType().Name;

            Assert.DoesNotThrow(() => App.WaitForNoElement(Trait, timeout: timeout), message);
        }


        public void NavigateTo(string tabName)
        {
            if (OnAndroid)
            {
                if (App.Query(hamburger).Any())
                    App.Tap(hamburger);

                App.Screenshot("Navigation Menu Open");
                int count = 0;
                while (!App.Query(tabName).Any() && count < 3)
                {
                    App.ScrollDown(x => x.Class("NavigationMenuView"));
                    count++;
                }
            }
            App.Tap(tab(tabName));
        }

        #region CommonPageActions

        // Use this region to define functionality that is common across many or all pages in your app.
        // Eg tapping the back button of a page or selecting the tabs of a tab bar

        Query hamburger;
        Func<string, Query> tab;

        void InitializeCommonQueries()
        {
            if (OnAndroid)
            {
                hamburger = x => x.Class("ImageButton").Marked("OK");
                tab = name => x => x.Class("NavigationMenuItemView").Text(name);
            }

            if (OniOS)
            {
                tab = name => x => x.Class("UITabBarButtonLabel").Text(name);
            }
        }

        #endregion
    }
}