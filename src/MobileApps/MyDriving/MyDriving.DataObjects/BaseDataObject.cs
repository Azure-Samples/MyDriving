// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using MvvmHelpers;
#if BACKEND
using Microsoft.Azure.Mobile.Server;

#endif

namespace MyDriving.DataObjects
{
    public interface IBaseDataObject
    {
        string Id { get; set; }
    }

#if BACKEND
    public class BaseDataObject : EntityData, IBaseDataObject
    {
        public BaseDataObject()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
#else
    public class BaseDataObject : ObservableObject, IBaseDataObject
    {
        public BaseDataObject()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
    }
#endif
}