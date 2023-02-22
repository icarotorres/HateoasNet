// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using HateoasNet.Abstractions;
using HateoasNet.Framework.Sample.Models;

namespace HateoasNet.Framework.Sample.HateoasBuilders
{
    public class InviteHateoasBuilder : IHateoasSourceBuilder<Invite>
    {
        public void Build(IHateoasSource<Invite> source)
        {
            _ = source.AddLink("accept-invite")
                    .HasRouteData(e => new { id = e.Id })
                    .When(e => e.Status == InviteStatuses.Pending);

            _ = source.AddLink("decline-invite")
                    .HasRouteData(e => new { id = e.Id })
                    .When(e => e.Status == InviteStatuses.Pending);

            _ = source.AddLink("cancel-invite")
                    .HasRouteData(e => new { id = e.Id })
                    .When(e => e.Status == InviteStatuses.Pending);

            _ = source.AddLink("get-invite").HasRouteData(i => new { id = i.Id });
            _ = source.AddLink("get-guild").HasRouteData(i => new { id = i.GuildId });
            _ = source.AddLink("get-member").HasRouteData(i => new { id = i.MemberId });
        }
    }
}
