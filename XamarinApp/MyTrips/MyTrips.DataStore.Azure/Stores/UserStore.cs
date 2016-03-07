using System;
using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;
namespace MyTrips.DataStore.Azure.Stores
{
    public class UserStore : BaseStore<UserProfile>, IUserStore
    {
    }
}

