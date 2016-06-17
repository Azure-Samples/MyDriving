// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyDriving.Utils
{
    public enum LoginAccount
    {
        None = 0,
        Facebook = 1,
        Microsoft = 2,
        Twitter = 3,
    }

    /// <summary>
    ///     This is the Settings static class that can be used in your Core solution or in any
    ///     of your client applications. All settings are laid out the same exact way with getters
    ///     and setters.
    /// </summary>
    public class Settings : INotifyPropertyChanged
    {
        const string DatabaseIdKey = "azure_database";

        const string LoginAccountKey = "login_account";


        const string DeviceIdKey = "device_id";

        const string HostNameKey = "host_name";

        const string DeviceConnectionStringKey = "device_connection_string";


        const string MetricDistanceKey = "metric_distance";


        const string MetricUnitsKey = "metric_units";


        const string FirstRunKey = "first_run";

        const string LoginAttemptsKey = "login_attempts";
        const int LoginAttemptsDefault = 0;

        const string HasSyncedDataKey = "has_synced";
        const bool HasSyncedDataDefault = false;

        const string LastSyncKey = "last_sync";

        static Settings _settings;
        static readonly int DatabaseIdDefault = 0;
        static readonly LoginAccount LoginAccountDefault = LoginAccount.None;
        static readonly string DeviceIdDefault = string.Empty;
        static readonly string HostNameDefault = string.Empty;
        static readonly string DeviceConnectionStringDefault = string.Empty;
        static readonly bool MetricDistanceDefault = false;
        static readonly bool MetricUnitsDefault = false;
        static readonly bool FirstRunDefault = true;
        static readonly DateTime LastSyncDefault = DateTime.Now.AddDays(-30);

        bool isConnected;

        static ISettings AppSettings => CrossSettings.Current;

        /// <summary>
        ///     Gets or sets the current settings. This should always be used
        /// </summary>
        /// <value>The current.</value>
        public static Settings Current => _settings ?? (_settings = new Settings());

        public int DatabaseId
        {
            get { return AppSettings.GetValueOrDefault(DatabaseIdKey, DatabaseIdDefault); }
            set { AppSettings.AddOrUpdateValue(DatabaseIdKey, value); }
        }


        public LoginAccount LoginAccount
        {
            get { return (LoginAccount) AppSettings.GetValueOrDefault(LoginAccountKey, (int) LoginAccountDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue(LoginAccountKey, (int) value))
                    OnPropertyChanged();
            }
        }

        public string DeviceId
        {
            get { return AppSettings.GetValueOrDefault(DeviceIdKey, DeviceIdDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue(DeviceIdKey, value))
                    OnPropertyChanged();
            }
        }

        public string HostName
        {
            get { return AppSettings.GetValueOrDefault(HostNameKey, HostNameDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue(HostNameKey, value))
                {
                    //if hostname is changed, DeviceConnectionString must be recreated
                    DeviceConnectionString = string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        public string DeviceConnectionString
        {
            get { return AppSettings.GetValueOrDefault(DeviceConnectionStringKey, DeviceConnectionStringDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue(DeviceConnectionStringKey, value))
                    OnPropertyChanged();
            }
        }

        public bool MetricDistance
        {
            get { return AppSettings.GetValueOrDefault(MetricDistanceKey, MetricDistanceDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue(MetricDistanceKey, value))
                    OnPropertyChanged();
            }
        }

        public bool MetricUnits
        {
            get { return AppSettings.GetValueOrDefault(MetricUnitsKey, MetricUnitsDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue(MetricUnitsKey, value))
                    OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the user wants to see favorites only.
        /// </summary>
        /// <value><c>true</c> if favorites only; otherwise, <c>false</c>.</value>
        public bool FirstRun
        {
            get { return AppSettings.GetValueOrDefault(FirstRunKey, FirstRunDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue(FirstRunKey, value))
                    OnPropertyChanged();
            }
        }

        public bool IsLoggedIn
            => !string.IsNullOrWhiteSpace(AuthToken) && !string.IsNullOrWhiteSpace(AzureMobileUserId);

        public int LoginAttempts
        {
            get { return AppSettings.GetValueOrDefault(LoginAttemptsKey, LoginAttemptsDefault); }
            set { AppSettings.AddOrUpdateValue(LoginAttemptsKey, value); }
        }

        public bool HasSyncedData
        {
            get { return AppSettings.GetValueOrDefault(HasSyncedDataKey, HasSyncedDataDefault); }
            set { AppSettings.AddOrUpdateValue(HasSyncedDataKey, value); }
        }

        public DateTime LastSync
        {
            get { return AppSettings.GetValueOrDefault(LastSyncKey, LastSyncDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue(LastSyncKey, value))
                    OnPropertyChanged();
            }
        }

        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                if (isConnected == value)
                    return;
                isConnected = value;
                OnPropertyChanged();
            }
        }

        public int UpdateDatabaseId()
        {
            return DatabaseId++;
        }

        #region User Profile

        const string UserUIDKey = "user_uid";
        static readonly string UserUIDDefault = string.Empty;

        public string UserUID
        {
            get { return AppSettings.GetValueOrDefault(UserUIDKey, UserUIDDefault); }
            set { AppSettings.AddOrUpdateValue(UserUIDKey, value); }
        }

        const string AzureMobileUserIdKey = "user_azure_id";
        static readonly string AzureMobileUserIdDefault = string.Empty;

        public string AzureMobileUserId
        {
            get { return AppSettings.GetValueOrDefault(AzureMobileUserIdKey, AzureMobileUserIdDefault); }
            set { AppSettings.AddOrUpdateValue(AzureMobileUserIdKey, value); }
        }

        const string AuthTokenKey = "authtoken";
        static readonly string AuthTokenDefault = string.Empty;

        public string AuthToken
        {
            get { return AppSettings.GetValueOrDefault(AuthTokenKey, AuthTokenDefault); }
            set { AppSettings.AddOrUpdateValue(AuthTokenKey, value); }
        }

        const string FirstNameKey = "user_firstname";
        static readonly string FirstNameDefault = string.Empty;

        public string UserFirstName
        {
            get { return AppSettings.GetValueOrDefault(FirstNameKey, FirstNameDefault); }
            set { AppSettings.AddOrUpdateValue(FirstNameKey, value); }
        }

        const string LastNameKey = "user_lastname";
        static readonly string LastNameDefault = string.Empty;

        public string UserLastName
        {
            get { return AppSettings.GetValueOrDefault(LastNameKey, LastNameDefault); }
            set { AppSettings.AddOrUpdateValue(LastNameKey, value); }
        }


        const string ProfileUrlKey = "user_profile_url";
        static readonly string ProfileUrlDefault = string.Empty;

        public string UserProfileUrl
        {
            get { return AppSettings.GetValueOrDefault(ProfileUrlKey, ProfileUrlDefault); }
            set { AppSettings.AddOrUpdateValue(ProfileUrlKey, value); }
        }


        public void Logout()
        {
            AuthToken = string.Empty;
            UserProfileUrl = string.Empty;
            UserFirstName = string.Empty;
            UserLastName = string.Empty;
            AzureMobileUserId = string.Empty;
            UserUID = string.Empty;
            LoginAccount = LoginAccount.None;
        }

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion
    }
}