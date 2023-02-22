// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using HateoasNet.Abstractions;

namespace HateoasNet
{
    public class HateoasSampleBuilder : IHateoasSourceBuilder<HateoasSample>
    {
        public void Build(IHateoasSource<HateoasSample> source)
        {
            _ = source.AddLink("test1").When(x => x.Id != Guid.Empty).HasRouteData(x => new { id = x.Id });
            _ = source.AddLink("test2").When(x => !string.IsNullOrWhiteSpace(x.ZipCode)).HasRouteData(x => new { zipcode = x.ZipCode });
        }
    }
}
