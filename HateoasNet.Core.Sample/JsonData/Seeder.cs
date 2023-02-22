// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HateoasNet.Core.Sample.JsonData
{
    public class Seeder
    {
        internal List<T> Seed<T>() where T : class
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            using var stream = new StreamReader($"JsonData\\{typeof(T).Name.ToLower()}s.json");
            return JsonConvert.DeserializeObject<List<T>>(stream.ReadToEnd(), serializerSettings);
        }
    }
}
