// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using HateoasNet.Abstractions;
using HateoasNet.Framework.Sample.Models;

namespace HateoasNet.Framework.Sample.HateoasBuilders
{
    public class ListMemberHateoasBuilder : IHateoasSourceBuilder<List<Member>>
    {
        public void Build(IHateoasSource<List<Member>> source)
        {
            _ = source.AddLink("get-members");
            _ = source.AddLink("invite-member");
            _ = source.AddLink("create-member");
        }
    }
}
