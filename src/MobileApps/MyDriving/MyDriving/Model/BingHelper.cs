// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace MyDriving.Model
{
    public static class BingHelper
    {
        private const string key = "ab8+c8el8rpCQokBCEhfBM5HbKiMZvGZ0Fs4Tz1svew";

        public static async Task<string> QueryBingImages(string city, double latitude, double longitude)
        {
            var bingQuery =
                $"https://api.datamarket.azure.com/Bing/Search/v1/Image?Query=%27{city}%27&Adult=%27Strict%27&Latitude={latitude}&Longitude={longitude}&ImageFilters=%27Size%3AMedium%2BAspect%3AWide%27&$top=1&$format=json";
            var handler = new HttpClientHandler
            {
                Credentials = new System.Net.NetworkCredential(key, key)
            };
            try
            {
                using (var client = new HttpClient(handler))
                {
                    var result = await client.GetStringAsync(bingQuery);

                    if (string.IsNullOrWhiteSpace(result))
                        return null;

                    var items = JsonConvert.DeserializeObject<BingQuery>(result).Query.Results;
                    if (items.Count > 0)
                        return items[0].MediaUrl;
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }
    }
}