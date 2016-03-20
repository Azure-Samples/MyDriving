// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using IoTHubPartitionMap.Interfaces;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceFabric.Actors;

namespace IoTHubPartitionMap
{
    internal class IoTHubPartitionMap : StatefulActor<IoTHubPartitionMap.ActorState>, IIoTHubPartitionMap
    {
        IActorTimer mTimer;

        Task<string> IIoTHubPartitionMap.LeaseTHubPartitionAsync()
        {
            string ret = "";
            foreach (string partition in State.PartitionNames)
            {
                if (!State.PartitionLeases.ContainsKey(partition))
                {
                    State.PartitionLeases.Add(partition, DateTime.Now);
                    ret = partition;
                    break;
                }
            }
            return Task.FromResult(ret);
        }

        Task<string> IIoTHubPartitionMap.RenewIoTHubPartitionLeaseAsync(string partition)
        {
            string ret = "";
            if (State.PartitionLeases.ContainsKey(partition))
            {
                State.PartitionLeases[partition] = DateTime.Now;
                ret = partition;
            }
            return Task.FromResult(ret);
        }

        protected override Task OnActivateAsync()
        {
            if (State == null)
            {
                State = new ActorState
                {
                    PartitionNames = new List<string>(),
                    PartitionLeases = new Dictionary<string, DateTime>()
                };
                ResetPartitionNames();
                mTimer = RegisterTimer(CheckLease, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
            }
            return Task.FromResult(true);
        }

        private Task CheckLease(Object state)
        {
            List<string> keys = State.PartitionLeases.Keys.ToList();

            foreach (string key in keys)
            {
                if (DateTime.Now - State.PartitionLeases[key] >= TimeSpan.FromSeconds(60))
                    State.PartitionLeases.Remove(key);
            }
            return Task.FromResult(1);
        }

        protected override Task OnDeactivateAsync()
        {
            if (mTimer != null)
                UnregisterTimer(mTimer);
            return base.OnDeactivateAsync();
        }

        void ResetPartitionNames()
        {
            var configSection = ActorService.ServiceInitializationParameters.CodePackageActivationContext
                .GetConfigurationPackageObject("Config");
            var conStr =
                configSection.Settings.Sections["ServiceConfigSection"].Parameters["EventHubConnectionString"].Value;
            var eventHubName = configSection.Settings.Sections["ServiceConfigSection"].Parameters["EventHubName"].Value;
            var eventHubClient = EventHubClient.CreateFromConnectionString(conStr, eventHubName);
            var partitions = eventHubClient.GetRuntimeInformation().PartitionIds;
            foreach (string partition in partitions)
            {
                State.PartitionNames.Add(partition);
            }
        }

        [DataContract]
        internal sealed class ActorState
        {
            [DataMember]
            public List<string> PartitionNames { get; set; }

            [DataMember]
            public Dictionary<string, DateTime> PartitionLeases { get; set; }
        }
    }
}