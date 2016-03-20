// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Fabric;
using System.Threading;
using Microsoft.ServiceFabric.Actors;

namespace IoTHubPartitionMap
{
    internal static class Program
    {
        /// <summary>
        ///     This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // Creating a FabricRuntime connects this host process to the Service Fabric runtime on this node.
                using (FabricRuntime fabricRuntime = FabricRuntime.Create())
                {
                    // This line registers your actor class with the Fabric Runtime.
                    // The contents of your ServiceManifest.xml and ApplicationManifest.xml files
                    // are automatically populated when you build this project.
                    // For more information, see http://aka.ms/servicefabricactorsplatform
                    fabricRuntime.RegisterActor<IoTHubPartitionMap>();

                    Thread.Sleep(Timeout.Infinite);
                    // Prevents this host process from terminating to keep the service host process running.
                }
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}