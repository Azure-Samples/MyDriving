// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MyDriving.Interfaces;
using MyDriving.Shared;
using MyDriving.Utils;
using MyDriving.UWP.Helpers;
using MyDriving.UWP.Views;
using MyDriving.Utils.Interfaces;

namespace MyDriving.UWP
{
    /// <summary>
    ///     Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {

        /// <summary>
        ///     Initializes the singleton application object.  This is the first line of authored code
        ///     executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;

            ViewModel.ViewModelBase.Init();

            // Added the AppId's provided by Thomas Dohmke. 
            Microsoft.HockeyApp.HockeyClient.Current.Configure(Logger.HockeyAppUWP);
        }

        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            SplitViewShell shell = Window.Current.Content as SplitViewShell;

            // Do not repeat app initialization when the Window is already present,
            // just ensure that the window is active
            if (shell == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                Frame rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                //Register platform specific implementations of shared interfaces
                ServiceLocator.Instance.Add<IAuthentication, Authentication>();
                ServiceLocator.Instance.Add<Utils.Interfaces.ILogger, PlatformLogger>();
                ServiceLocator.Instance.Add<IOBDDevice, OBDDevice>();

                if (Settings.Current.IsLoggedIn)
                {
                    //When the first screen of the app is launched after user has logged in, initialize the processor that manages connection to OBD Device and to the IOT Hub
                    MyDriving.Services.OBDDataProcessor.GetProcessor().Initialize(ViewModel.ViewModelBase.StoreManager);

                    // Create the shell and set it to current trip
                    shell = new SplitViewShell(rootFrame);
                    shell.SetTitle("CURRENT TRIP");
                    shell.SetSelectedPage("CURRENT TRIP");
                    rootFrame.Navigate(typeof(CurrentTripView), e.Arguments);
                    Window.Current.Content = shell;
                }
                else if (Settings.Current.FirstRun)
                {
                    rootFrame.Navigate(typeof(GetStarted1), e.Arguments);
                    Window.Current.Content = rootFrame;
                }
                else
                {
                    rootFrame.Navigate(typeof(LoginView), e.Arguments);
                    Window.Current.Content = rootFrame;
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }


        /// <summary>
        ///     Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        ///     Invoked when application execution is being suspended.  Application state is saved
        ///     without knowing whether the application will be terminated or resumed with the contents
        ///     of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        public static void SetTitle(string title)
        {
            SplitViewShell shell = Window.Current.Content as SplitViewShell;
            if (shell != null)
            {
                shell.SetTitle(title);
                shell.SetSelectedPage(title);
            }
        }
   
    }
}