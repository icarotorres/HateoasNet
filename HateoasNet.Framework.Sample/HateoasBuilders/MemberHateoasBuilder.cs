// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using HateoasNet.Abstractions;
using HateoasNet.Framework.Sample.Models;

namespace HateoasNet.Framework.Sample.HateoasBuilders
{
    public class MemberHateoasBuilder : IHateoasSourceBuilder<Member>
    {
        public void Build(IHateoasSource<Member> source)
        {
            _ = source.AddLink("get-member").HasRouteData(e => new { id = e.Id });
            _ = source.AddLink("update-member").HasRouteData(e => new { id = e.Id });

            _ = source
                .AddLink("get-guild")
                .HasRouteData(e => new { id = e.GuildId })
                .When(e => e.GuildId != null);

            _ = source
                .AddLink("promote-member")
                .HasRouteData(e => new { id = e.Id })
                .When(e => e.GuildId != null && !e.IsGuildMaster);

            _ = source
                .AddLink("demote-member")
                .HasRouteData(e => new { id = e.Id })
                .When(e => e.GuildId != null && e.IsGuildMaster);

            _ = source.AddLink("leave-guild")
                    .HasRouteData(e => new { id = e.Id })
                    .When(e => e.GuildId != null);
        }
    }
}
