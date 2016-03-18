// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof (MyDrivingService.Startup))]

namespace MyDrivingService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}