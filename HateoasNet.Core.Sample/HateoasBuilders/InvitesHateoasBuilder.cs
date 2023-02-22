// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using HateoasNet.Abstractions;
using HateoasNet.Core.Sample.Models;

namespace HateoasNet.Core.Sample.HateoasBuilders
{
    public class InvitesHateoasBuilder : IHateoasSourceBuilder<List<Invite>>
    {
        public void Build(IHateoasSource<List<Invite>> source)
        {
            _ = source.AddLink("get-invites");
            _ = source.AddLink("invite-member");
        }
    }
}
