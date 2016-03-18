// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyDriving.Model
{
    public class Thumbnail
    {
        public string MediaUrl { get; set; }
        public string ContentType { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string FileSize { get; set; }
    }

    public class BingImage
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string MediaUrl { get; set; }
        public string SourceUrl { get; set; }
        public string DisplayUrl { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string FileSize { get; set; }
        public string ContentType { get; set; }
        public Thumbnail Thumbnail { get; set; }
    }

    public class BingQueryResults
    {
        [JsonProperty("results")]
        public List<BingImage> Results { get; set; }
    }

    public class BingQuery
    {
        [JsonProperty("d")]
        public BingQueryResults Query { get; set; }
    }
}