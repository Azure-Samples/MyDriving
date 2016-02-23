using System;
using MyTrips.DataObjects;
using MyTrips.DataStore.Abstractions;
using System.Threading.Tasks;

namespace MyTrips.DataStore.Mock.Stores
{
    public class FeedbackStore : BaseStore<Feedback>, IFeedbackStore
    {
        public override Task InitializeStoreAsync()
        {
            return Task.FromResult(true);
        }
    }
}

