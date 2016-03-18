// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using MyDriving.DataObjects;
using MyDriving.DataStore.Abstractions;

namespace MyDriving.DataStore.Azure.Stores
{
    public class UserStore : BaseStore<UserProfile>, IUserStore
    {
        public override string Identifier => "User";
    }
}