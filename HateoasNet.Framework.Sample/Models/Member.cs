// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Newtonsoft.Json;

namespace HateoasNet.Framework.Sample.Models
{
    public class Member
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public bool IsGuildMaster { get; set; }
        public Guid? GuildId { get; set; }
        [JsonIgnore] public Guild Guild { get; set; }
    }
}
