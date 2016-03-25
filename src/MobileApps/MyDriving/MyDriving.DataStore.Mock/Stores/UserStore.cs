// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using MyDriving.DataObjects;
using MyDriving.DataStore.Abstractions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MyDriving.DataStore.Mock.Stores
{
    public class UserStore : BaseStore<UserProfile>, IUserStore
    {
		public override Task<UserProfile> GetItemAsync(string id)
		{
			var profile = new UserProfile
			{
				FirstName = "Scott",
				LastName = "Gu",
				ProfilePictureUri = "http://refractored.com/images/Scott.png",
				HardAccelerations = 32,
				HardStops = 12,
				Rating = 98,
				TotalDistance = 99,
				MaxSpeed = 55,
				FuelConsumption = 10,
				TotalTime = 60 * 90,
				TotalTrips = 8,
				UserId = "1"
			};

			return Task.FromResult (profile);
		}

        public override Task<IEnumerable<UserProfile>> GetItemsAsync(int skip = 0, int take = 100,
            bool forceRefresh = false)
        {
            var items = new List<UserProfile>
            {
                new UserProfile
                {
                    FirstName = "Scott",
                    LastName = "Gu",
                    ProfilePictureUri = "http://refractored.com/images/Scott.png",
                    HardAccelerations = 55,
                    HardStops = 2,
                    Rating = 98,
                    TotalDistance = 99,
                    MaxSpeed = 55,
                    FuelConsumption = 10,
                    TotalTime = 60*90,
                    TotalTrips = 8,
                    UserId = "1"
                }
            };

            return Task.FromResult(items as IEnumerable<UserProfile>);
        }
    }
}