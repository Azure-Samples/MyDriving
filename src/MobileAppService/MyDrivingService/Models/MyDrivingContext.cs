// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Microsoft.Azure.Mobile.Server.Tables;
using MyDriving.DataObjects;

namespace MyDrivingService.Models
{
    public class MyDrivingContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to alter your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        private const string ConnectionStringName = "Name=MS_TableConnectionString";

        public MyDrivingContext() : base(ConnectionStringName)
        {
        }

        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripPoint> TripPoints { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<IOTHubData> IOTHubDatas { get; set; }
        public DbSet<POI> POIs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));
        }
    }
}