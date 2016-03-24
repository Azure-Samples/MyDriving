// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using MyDriving.DataObjects;
using MyDriving.DataStore.Abstractions;
using System.Linq;

namespace MyDriving.DataStore.Azure.Stores
{
    public class UserStore : BaseStore<UserProfile>, IUserStore
    {
        public override string Identifier => "User";

        public override async System.Threading.Tasks.Task<UserProfile> GetItemAsync(string id)
        {
            var users = await base.GetItemsAsync(0, 10, true);
            return users.FirstOrDefault(s => s.UserId == id);
        }
    }
}