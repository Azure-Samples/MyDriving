using System;
using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;
using System.Threading.Tasks;
using MyTrips.Utils;
using System.Collections.Generic;

namespace MyTrips.DataStore.Mock.Stores
{
    public class UserStore : BaseStore<UserProfile>, IUserStore
    {
        public override Task<IEnumerable<UserProfile>> GetItemsAsync(int skip = 0, int take = 100, bool forceRefresh = false)
        {
            var items = new List<UserProfile>();
            items.Add(new UserProfile
            {
                FirstName = "Scott",
                LastName = "Gu",
                ProfilePictureUri = "http://refractored.com/images/Scott.png",
                AverageSpeed = 55,
                AverageFuelConsumption = 2,
                Rating = 98,
                TotalDistance = 99,
                TotalEmissions = 1,
                UserId = "1"
            });

            return Task.FromResult(items as IEnumerable<UserProfile>);
        }
    }
}

