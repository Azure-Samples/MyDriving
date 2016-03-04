// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyTrips.Utils
{
    public enum LoginAccount
    {
        None = 0,
        Facebook = 1,
        Microsoft = 2,
        Twitter = 3,
    }

    public enum UserPictureSourceKind
    {
        None = 0,
        Url,
        Byte
    }
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    /// </summary>
    public class Settings : INotifyPropertyChanged
    {
        static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        static Settings settings;

        /// <summary>
        /// Gets or sets the current settings. This should always be used
        /// </summary>
        /// <value>The current.</value>
        public static Settings Current
        {
            get { return settings ?? (settings = new Settings()); }
        }

        const string DatabaseIdKey = "azure_database";
        static readonly int DatabaseIdDefault = 0;

        public int DatabaseId
        {
            get { return AppSettings.GetValueOrDefault<int>(DatabaseIdKey, DatabaseIdDefault); }
            set
            {
                AppSettings.AddOrUpdateValue<int>(DatabaseIdKey, value);
            }
        }

        public int UpdateDatabaseId()
        {
            return DatabaseId++;
        }

        const string LoginAccountKey = "login_account";
        static readonly LoginAccount LoginAccountDefault = LoginAccount.None;


        public LoginAccount LoginAccount
        {
            get { return  (LoginAccount)AppSettings.GetValueOrDefault<int>(LoginAccountKey, (int)LoginAccountDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue<int>(LoginAccountKey, (int)value))
                    OnPropertyChanged();
            }
        }


        const string HubSetting1Key = "hub_setting_1";
        static readonly string HubSetting1Default = string.Empty;

        public string HubSetting1
        {
            get { return AppSettings.GetValueOrDefault<string>(HubSetting1Key, HubSetting1Default); }
            set
            {
                if (AppSettings.AddOrUpdateValue<string>(HubSetting1Key, value))
                    OnPropertyChanged();
            }
        }

        const string HubSetting2Key = "hub_setting_2";
        static readonly string HubSetting2Default = string.Empty;

        public string HubSetting2
        {
            get { return AppSettings.GetValueOrDefault<string>(HubSetting2Key, HubSetting2Default); }
            set
            {
                if (AppSettings.AddOrUpdateValue<string>(HubSetting2Key, value))
                    OnPropertyChanged();
            }
        }
            


        const string PushNotificationsEnabledKey = "push_enabled";
        static readonly bool PushNotificationsEnabledDefault = false;

        public bool PushNotificationsEnabled
        {
            get { return AppSettings.GetValueOrDefault<bool>(PushNotificationsEnabledKey, PushNotificationsEnabledDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue<bool>(PushNotificationsEnabledKey, value))
                    OnPropertyChanged();
            }
        }

        const string MetricDistanceKey = "metric_distance";
        static readonly bool MetricDistanceDefault = false;

        public bool MetricDistance
        {
            get { return AppSettings.GetValueOrDefault<bool>(MetricDistanceKey, MetricDistanceDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue<bool>(MetricDistanceKey, value))
                    OnPropertyChanged();
            }
        }


        const string MetricUnitsKey = "metric_units";
        static readonly bool MetricUnitsDefault = false;

        public bool MetricUnits
        {
            get { return AppSettings.GetValueOrDefault<bool>(MetricUnitsKey, MetricUnitsDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue<bool>(MetricUnitsKey, value))
                    OnPropertyChanged();
            }
        }

		const string CurrencyKey = "currency";
		static readonly string CurrencyDefault = "US (Dollar)";

		public string Currency
		{
			get { return AppSettings.GetValueOrDefault<string>(CurrencyKey, CurrencyDefault); }
			set
			{
				if (AppSettings.AddOrUpdateValue<string>(CurrencyKey, value))
					OnPropertyChanged();
			}
		}

        const string FirstRunKey = "first_run";
        static readonly bool FirstRunDefault = true;

        /// <summary>
        /// Gets or sets a value indicating whether the user wants to see favorites only.
        /// </summary>
        /// <value><c>true</c> if favorites only; otherwise, <c>false</c>.</value>
        public bool FirstRun
        {
            get { return AppSettings.GetValueOrDefault<bool>(FirstRunKey, FirstRunDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue<bool>(FirstRunKey, value))
                    OnPropertyChanged();
            }
        }

        public bool IsLoggedIn => (!string.IsNullOrWhiteSpace(AuthToken) && !string.IsNullOrWhiteSpace(UserId));

        const string LoginAttemptsKey = "login_attempts";
        const int LoginAttemptsDefault = 0;
        public int LoginAttempts
        {
            get
            {
                return AppSettings.GetValueOrDefault<int>(LoginAttemptsKey, LoginAttemptsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<int>(LoginAttemptsKey, value);
            }
        }

        const string HasSyncedDataKey = "has_synced";
        const bool HasSyncedDataDefault = false;
        public bool HasSyncedData
        {
            get { return AppSettings.GetValueOrDefault<bool>(HasSyncedDataKey, HasSyncedDataDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(HasSyncedDataKey, value); }

        }

        const string LastSyncKey = "last_sync";
        static readonly DateTime LastSyncDefault = DateTime.Now.AddDays(-30);
        public DateTime LastSync
        {
            get
            {
                return AppSettings.GetValueOrDefault<DateTime>(LastSyncKey, LastSyncDefault);
            }
            set
            {
                if (AppSettings.AddOrUpdateValue<DateTime>(LastSyncKey, value))
                    OnPropertyChanged();
            }
        }

        bool isConnected;
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

        #region User Profile
        const string UserIdKey = "userid";
        static readonly string UserIdDefault = string.Empty;
        public string UserId
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(UserIdKey, UserIdDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(UserIdKey, value);
            }
        }

        const string AuthTokenKey = "authtoken";
        static readonly string AuthTokenDefault = string.Empty;

        public string AuthToken
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(AuthTokenKey, AuthTokenDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(AuthTokenKey, value);
            }
        }

        const string FirstNameKey = "user_firstname";
        static readonly string FirstNameDefault = string.Empty;

        public string UserFirstName
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(FirstNameKey, FirstNameDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(FirstNameKey, value);
            }
        }

        const string LastNameKey = "user_lastname";
        static readonly string LastNameDefault = string.Empty;

        public string UserLastName
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(LastNameKey, LastNameDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(LastNameKey, value);
            }
        }

        const string ProfileUrlKey = "user_profile_url";
        static readonly string ProfileUrlDefault = string.Empty;

        public string UserProfileUrl
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(ProfileUrlKey, ProfileUrlDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(ProfileUrlKey, value);
            }
        }


        const string ProfileByteKey = "user_profile_byte";
        static readonly byte[] ProfileByteDefault = new byte[0];

        public byte[] UserProfileByteArr
        {
            get
            {
                return AppSettings.GetValueOrDefault<byte[]>(ProfileByteKey, ProfileByteDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<byte[]>(ProfileByteKey, value);
            }
        }


        const string UserPictureSourceKey = "picture_source";
        static readonly UserPictureSourceKind UserPictureSourceDefault = UserPictureSourceKind.None;

        public UserPictureSourceKind UserPictureSourceKind
        {
            get { return (UserPictureSourceKind)AppSettings.GetValueOrDefault<int>(UserPictureSourceKey, (int)UserPictureSourceDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue<int>(UserPictureSourceKey, (int)value))
                    OnPropertyChanged();
            }
        }

        public void CleanupUserProfile()
        {
            UserId = String.Empty;
            AuthToken = String.Empty;
            UserProfileUrl = String.Empty;
            UserFirstName = String.Empty;
            UserLastName = String.Empty;
            LoginAccount = LoginAccount.None;
            UserPictureSourceKind = UserPictureSourceKind.None;
            UserProfileByteArr = new byte[0];
        }
        #endregion


        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion
    }
}