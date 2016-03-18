// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.WindowsAzure.MobileServices;

namespace MyDriving.AzureClient
{
    public interface IAzureClient
    {
        IMobileServiceClient Client { get; }
    }
}