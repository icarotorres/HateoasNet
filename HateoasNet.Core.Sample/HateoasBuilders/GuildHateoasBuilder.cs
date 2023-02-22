// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using HateoasNet.Abstractions;
using HateoasNet.Core.Sample.Models;

namespace HateoasNet.Core.Sample.HateoasBuilders
{
    public class GuildHateoasBuilder : IHateoasSourceBuilder<Guild>
    {
        public void Build(IHateoasSource<Guild> source)
        {
            _ = source.AddLink("get-guild").HasRouteData(g => new { id = g.Id }).PresentedAs("getGuild");
            _ = source.AddLink("get-members").HasRouteData(g => new { guildId = g.Id });
            _ = source.AddLink("update-guild").HasRouteData(e => new { id = e.Id });
        }
    }
}
