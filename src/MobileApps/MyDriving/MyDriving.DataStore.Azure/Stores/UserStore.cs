using System;
using MyDriving.DataObjects;
using MyDriving.DataStore.Abstractions;
namespace MyDriving.DataStore.Azure.Stores
{
    public class UserStore : BaseStore<UserProfile>, IUserStore
    {
        public override string Identifier => "User";
    }
}

