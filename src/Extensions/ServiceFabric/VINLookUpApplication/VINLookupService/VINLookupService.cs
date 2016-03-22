// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IoTHubPartitionMap.Interfaces;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json;
using VINParser;

namespace VINLookupService
{
    /// <summary>
    ///     The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class VINLookupService : StatelessService
    {
        /// <summary>
        ///     Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            // TODO: If your service needs to handle user requests, return a list of ServiceReplicaListeners here.
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        ///     This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancelServiceInstance">Canceled when Service Fabric terminates this instance.</param>
        protected override async Task RunAsync(CancellationToken cancelServiceInstance)
        {
            var configSection = ServiceInitializationParameters.CodePackageActivationContext
                .GetConfigurationPackageObject("Config");
            var conStr =
                configSection.Settings.Sections["ServiceConfigSection"].Parameters["EventHubConnectionString"].Value;
            var eventHubName = configSection.Settings.Sections["ServiceConfigSection"].Parameters["EventHubName"].Value;
            var sqlConnectionString =
                configSection.Settings.Sections["ServiceConfigSection"].Parameters["SqlConnectionString"].Value;
            var eventHubClient = EventHubClient.CreateFromConnectionString(conStr, eventHubName);
            DateTime timeStamp = DateTime.Now;
            var proxy = ActorProxy.Create<IIoTHubPartitionMap>(new ActorId(1),
                ServiceInitializationParameters.CodePackageActivationContext.ApplicationName);
            var dataPackage = ServiceInitializationParameters.CodePackageActivationContext.GetDataPackageObject("Data");

            VINParser.VINParser parser = new VINParser.VINParser(Path.Combine(dataPackage.Path, "wmis.json"));

            while (!cancelServiceInstance.IsCancellationRequested)
            {
                string partition = proxy.LeaseTHubPartitionAsync().Result;
                if (partition == "")
                    await Task.Delay(TimeSpan.FromSeconds(15), cancelServiceInstance);
                else
                {
                    var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup()
                        .CreateReceiver(partition, DateTime.UtcNow);
                    while (!cancelServiceInstance.IsCancellationRequested)
                    {
                        EventData eventData = await eventHubReceiver.ReceiveAsync();
                        if (eventData != null)
                        {
                            string data = Encoding.UTF8.GetString(eventData.GetBytes());
                            TripVIN vin = JsonConvert.DeserializeObject<TripVIN>(data);
                            try
                            {
                                var carInfo = parser.Parse(vin.VIN);
                                SaveRecord(sqlConnectionString, vin.VIN, carInfo);
                            }
                            catch (Exception exp)
                            {
                                ServiceEventSource.Current.ServiceRequestFailed("VINLookupService",
                                    "Failed to Save VIN: " + exp.Message);
                            }
                        }
                        if (DateTime.Now - timeStamp > TimeSpan.FromSeconds(20))
                        {
                            string lease = proxy.RenewIoTHubPartitionLeaseAsync(partition).Result;
                            if (lease == "")
                                break;
                        }
                    }
                }
            }
        }

        private void SaveRecord(string sqlConnectionString, string vin, CarInfo carInfo)
        {
            using (var conn = new SqlConnection(sqlConnectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT COUNT(vinNum) AS RecCount FROM dbo.dimVinLookup WHERE vinNum = @vin";
                cmd.Parameters.AddWithValue("@vin", vin);
                conn.Open();
                int recCount = (int) cmd.ExecuteScalar();
                if (recCount == 0)
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandText = @"
                    INSERT dbo.dimVinLookup (vinNum, make, model, carYear, carType)
                    OUTPUT INSERTED.vinNum
                    VALUES (@vinNum, @make, @model, @carYear, @carType)";

                    cmd.Parameters.AddWithValue("@vinNum", vin);
                    cmd.Parameters.AddWithValue("@make", carInfo.Make);
                    cmd.Parameters.AddWithValue("@model", carInfo.Model);
                    cmd.Parameters.AddWithValue("@carYear", carInfo.Year);
                    cmd.Parameters.AddWithValue("@carType", carInfo.CarType.ToString());
                    string insertedId = (string) cmd.ExecuteScalar();
                    ServiceEventSource.Current.ServiceMessage(this, "Records ID {0} inserted.", insertedId);
                }
                else
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandText = @"
                    UPDATE dbo.dimVinLookup SET make=@make, model=@model, carYear=@carYear, carType=@carType
                    WHERE vinNum=@vinNum";

                    cmd.Parameters.AddWithValue("@vinNum", vin);
                    cmd.Parameters.AddWithValue("@make", carInfo.Make);
                    cmd.Parameters.AddWithValue("@model", carInfo.Model);
                    cmd.Parameters.AddWithValue("@carYear", carInfo.Year);
                    cmd.Parameters.AddWithValue("@carType", carInfo.CarType.ToString());

                    int rowCount = cmd.ExecuteNonQuery();
                    ServiceEventSource.Current.ServiceMessage(this, "{0} row updated.", rowCount);
                }
            }
        }
    }
}