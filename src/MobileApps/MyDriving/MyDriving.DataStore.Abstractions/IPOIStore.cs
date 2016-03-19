// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using MyDriving.DataObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyDriving.DataStore.Abstractions
{
    public interface IPOIStore : IBaseStore<POI>
    {
        Task<IEnumerable<POI>> GetItemsAsync(string tripId);
    }
}