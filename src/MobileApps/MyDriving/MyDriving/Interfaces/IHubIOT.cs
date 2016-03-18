// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyDriving.Interfaces
{
    public interface IHubIOT
    {
        void Initialize(string connectionStr);
        Task SendEvents(IEnumerable<String> blobs);

        Task SendEvent(string blob);
    }
}