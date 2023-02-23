// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using HateoasNet.Abstractions;
using HateoasNet.Extensions;

namespace HateoasNet.Infrastructure
{
    /// <inheritdoc cref="IHateoasLinkBuilder{T}" />
    internal sealed class HateoasLinkBuilder<T> : IHateoasLinkBuilder<T> where T : class
    {
        internal HateoasLinkBuilder(string routeName) : this(routeName, _ => null, _ => true)
        {
        }

        private HateoasLinkBuilder(string routeName, Func<T, IDictionary<string, object>> routeDictionaryFunction, Func<T, bool> predicate)
        {
            RouteName = routeName;
            PresentedName = routeName;
            RouteDictionaryFunction = routeDictionaryFunction;
            Predicate = predicate;
        }

        public string RouteName { get; }
        public string PresentedName { get; private set; }

        internal Func<T, bool> Predicate { get; private set; }
        Func<T, bool> IHateoasLinkBuilder<T>.Predicate => Predicate;


        internal Func<T, IDictionary<string, object>> RouteDictionaryFunction { get; private set; }
        Func<T, IDictionary<string, object>> IHateoasLinkBuilder<T>.RouteDictionaryFunction => RouteDictionaryFunction;

        IDictionary<string, object> IHateoasLinkBuilder.GetRouteDictionary(object source)
        {
            return source == null ? throw new ArgumentNullException(nameof(source)) : RouteDictionaryFunction((T)source);
        }

        public bool IsSatisfiedBy(object source)
        {
            return source == null ? throw new ArgumentNullException(nameof(source)) : Predicate((T)source);
        }

        public IHateoasLinkBuilder<T> HasRouteData(Func<T, object> routeDataFunction)
        {
            if (routeDataFunction == null)
            {
                throw new ArgumentNullException(nameof(routeDataFunction));
            }

            RouteDictionaryFunction = source => routeDataFunction(source).ToRouteDictionary();
            return this;
        }

        public IHateoasLinkBuilder<T> When(Func<T, bool> predicate)
        {
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            return this;
        }

        public IHateoasLinkBuilder<T> PresentedAs(string presentedName)
        {
            if (!string.IsNullOrWhiteSpace(presentedName))
            {
                PresentedName = presentedName;
            }

            return this;
        }
    }
}
