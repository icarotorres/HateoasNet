﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using Newtonsoft.Json;

namespace HateoasNet.Framework.Sample.JsonData
{
    public class Seeder
    {
        internal List<T> Seed<T>() where T : class
        {
            var filepath = HostingEnvironment.MapPath($"~/JsonData/{typeof(T).Name.ToLower()}s.json");
            using var stream = new StreamReader(filepath);
            return JsonConvert.DeserializeObject<List<T>>(stream.ReadToEnd());
        }
    }
}
