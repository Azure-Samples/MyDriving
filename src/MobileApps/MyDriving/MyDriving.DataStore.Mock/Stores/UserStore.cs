using System;
using MyDriving.DataObjects;
using MyDriving.DataStore.Abstractions;
using System.Threading.Tasks;
using MyDriving.Utils;
using System.Collections.Generic;

namespace MyDriving.DataStore.Mock.Stores
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
                HardAccelerations = 55,
                HardStops = 2,
                Rating = 98,
                TotalDistance = 99,
                MaxSpeed = 55,
                FuelConsumption = 10,
                TotalTime = 60 * 90,
                TotalTrips = 8,
                UserId = "1"
            });

            return Task.FromResult(items as IEnumerable<UserProfile>);
        }
    }
}

