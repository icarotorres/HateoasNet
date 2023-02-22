// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace HateoasNet.Core.Sample.CustomHateoas
{
    public class DictionaryHateoas : AbstractHateoas<ImmutableDictionary<string, object>>
    {
        public DictionaryHateoas(IHateoas hateoas) : base(hateoas)
        {
        }

        protected override ImmutableDictionary<string, object> GenerateCustom(IEnumerable<HateoasLink> links)
        {
            return links.ToImmutableDictionary(x => x.Rel, x => (object)new { x.Href, x.Method });
        }
    }
}
