// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;

namespace VINParser
{
    public class MakeMap
    {
        public int StartDigit { get; set; }
        public int MaxLength { get; set; }
        public Dictionary<string, CarInfo> Map { get; set; }
    }
}