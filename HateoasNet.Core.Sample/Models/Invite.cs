// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.Json.Serialization;

namespace HateoasNet.Core.Sample.Models
{
    public class Invite
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public InviteStatuses Status { get; set; } = InviteStatuses.Pending;
        public Guid GuildId { get; set; }
        public Guid MemberId { get; set; }
        [JsonIgnore] public Member Member { get; set; }
        [JsonIgnore] public Guild Guild { get; set; }
    }

    public enum InviteStatuses : short
    {
        /// <summary>
        ///   Waiting for an answer as accepted, declined or canceled
        /// </summary>
        Pending = 1,

        /// <summary>
        ///   Accepted by invited member
        /// </summary>
        Accepted,

        /// <summary>
        ///   Declined by invited member
        /// </summary>
        Declined,

        /// <summary>
        ///   Canceled by inviting guild
        /// </summary>
        Canceled
    }
}
