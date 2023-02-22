// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using HateoasNet.Core.Sample.HateoasBuilders;
using HateoasNet.Core.Sample.Models;
using HateoasNet.DependencyInjection.Core;
using Microsoft.Extensions.DependencyInjection;

namespace HateoasNet.Core.Sample
{
    public static class HateoasSetupExtensions
    {
        public static IServiceCollection HateoasConfigurationUsingAssembly(this IServiceCollection services)
        {
            // setup applying map configurations from classes in separated files found in a given assembly
            return services.ConfigureHateoas(context => context.ConfigureFromAssembly(typeof(GuildHateoasBuilder).Assembly));
        }

        public static IServiceCollection HateoasConfigurations(this IServiceCollection services)
        {
            // setup applying map configurations from classes in separated files
            return services.ConfigureHateoas(context => context
                .ApplyConfiguration(new GuildHateoasBuilder())
                .ApplyConfiguration(new ListGuildHateoasBuilder())
                .ApplyConfiguration(new MemberHateoasBuilder())
                .ApplyConfiguration(new ListMemberHateoasBuilder())
                .ApplyConfiguration(new InviteHateoasBuilder())
                .ApplyConfiguration(new InvitesHateoasBuilder()));
        }

        public static IServiceCollection HateoasInlineConfiguration(this IServiceCollection services)
        {
            return services.ConfigureHateoas(context =>
            {
                return context
                       // map All Api returns of type List<Guild> to links with no routeData and no conditional predicate
                       .Configure<List<Guild>>(guilds =>
                       {
                           _ = guilds.AddLink("get-guilds");
                           _ = guilds.AddLink("create-guild");
                       })

                       // map All Api returns of type List<Member> to links with no routeData and no conditional predicate
                       .Configure<List<Member>>(members =>
                       {
                           _ = members.AddLink("get-members");
                           _ = members.AddLink("invite-member");
                           _ = members.AddLink("create-member");
                       })

                       // map type with links for Pagination with no routeData and no conditional predicate
                       .Configure<List<Invite>>(invites =>
                       {
                           _ = invites.AddLink("get-invites");
                           _ = invites.AddLink("invite-member");
                       })

                       // map type with links for single objects
                       .Configure<Guild>(guild =>
                       {
                           _ = guild.AddLink("get-guild").HasRouteData(g => new { id = g.Id });
                           _ = guild.AddLink("get-members").HasRouteData(g => new { guildId = g.Id });
                           _ = guild.AddLink("update-guild").HasRouteData(e => new { id = e.Id });
                       })
                       .Configure<Invite>(invite =>
                       {
                           _ = invite.AddLink("accept-invite")
                                 .HasRouteData(e => new { id = e.Id })
                                 .When(e => e.Status == InviteStatuses.Pending);

                           _ = invite.AddLink("decline-invite")
                                 .HasRouteData(e => new { id = e.Id })
                                 .When(e => e.Status == InviteStatuses.Pending);

                           _ = invite.AddLink("cancel-invite")
                                 .HasRouteData(e => new { id = e.Id })
                                 .When(e => e.Status == InviteStatuses.Pending);

                           _ = invite.AddLink("get-invite").HasRouteData(i => new { id = i.Id });
                           _ = invite.AddLink("get-guild").HasRouteData(i => new { id = i.GuildId });
                           _ = invite.AddLink("get-member").HasRouteData(i => new { id = i.MemberId });
                       })
                       .Configure<Member>(member =>
                       {
                           _ = member.AddLink("get-member").HasRouteData(e => new { id = e.Id });
                           _ = member.AddLink("update-member").HasRouteData(e => new { id = e.Id });
                       })
                       .Configure<Member>(member =>
                       {
                           _ = member
                               .AddLink("get-guild")
                               .HasRouteData(e => new { id = e.GuildId })
                               .When(e => e.GuildId != null);

                           _ = member
                               .AddLink("promote-member")
                               .HasRouteData(e => new { id = e.Id })
                               .When(e => e.GuildId != null && !e.IsGuildMaster);

                           _ = member
                               .AddLink("demote-member")
                               .HasRouteData(e => new { id = e.Id })
                               .When(e => e.GuildId != null && e.IsGuildMaster);

                           _ = member.AddLink("leave-guild")
                                 .HasRouteData(e => new { id = e.Id })
                                 .When(e => e.GuildId != null);
                       });
            });
        }
    }
}
