// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NETCOREAPP3_1
namespace HateoasNet.DependencyInjection.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using HateoasNet.Abstractions;
    using HateoasNet.Infrastructure;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.Extensions.DependencyInjection;

    [ExcludeFromCodeCoverage]
    internal static class CustomHateoasExtensions
    {
        internal static IEnumerable<(TypeInfo, Type)> GetServiceTuplesFromAssemblies(this Assembly[] assemblies)
        {
            return assemblies.SelectMany(a => a.DefinedTypes
                    .Select(t => (t, t.GetInterfaces().SingleOrDefault(IsCustomHateoas)))
                    .Where(x => x.Item2 != null));
        }

        internal static bool IsCustomHateoas(Type type)
        {
            return type.IsInterface && type.Name.Contains(typeof(IHateoas<>).Name);
        }
    }

    [ExcludeFromCodeCoverage]
    public static class HateoasExtensions
    {
        /// <summary>
        ///   Configure Hateoas Source mapping in .Net Core Web Api and register required services to
        ///   .net core default dependency injection container
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> being configured on Ioc container.</param>
        /// <param name="hateoasOptions">Function callback to configure <see cref="IHateoasContext"/> options.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection ConfigureHateoas(this IServiceCollection services, Func<IHateoasContext, IHateoasContext> hateoasOptions)
        {
            return hateoasOptions == null
                ? throw new ArgumentNullException(nameof(hateoasOptions))
                : services
                   .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                   .AddSingleton<IUrlHelperFactory, UrlHelperFactory>()
                   .AddScoped(x => hateoasOptions(new HateoasContext()))
                   .AddScoped(x => x.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(x.GetRequiredService<IActionContextAccessor>().ActionContext))
                   .AddScoped<IHateoas, Hateoas>();
        }

        public static IServiceCollection RegisterAllCustomHateoas(this IServiceCollection services, Assembly[] assemblies, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            foreach (var (implementation, abstraction) in assemblies.GetServiceTuplesFromAssemblies())
            {
                services.Add(new ServiceDescriptor(abstraction, implementation, lifetime));
            }

            return services;
        }
    }
}
#endif
