// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using MyDriving.DataObjects;
using MyDriving.DataStore.Abstractions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MyDriving.DataStore.Azure.Stores
{
    public class PhotoStore : BaseStore<Photo>, IPhotoStore
    {
        public override string Identifier => "Photo";

        public override Task<bool> SyncAsync()
        {
            return Task.FromResult(true);
        }

        public Task<IEnumerable<Photo>> GetTripPhotos(string tripId)
        {
            return Table.Where(s => s.TripId == tripId).ToEnumerableAsync();
        }
    }
}