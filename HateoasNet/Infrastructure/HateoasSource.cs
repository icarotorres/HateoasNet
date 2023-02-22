// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using HateoasNet.Abstractions;

namespace HateoasNet.Infrastructure
{
    /// <inheritdoc cref="IHateoasSource" />
    public sealed class HateoasSource<T> : IHateoasSource<T> where T : class
    {
        private readonly List<IHateoasLinkBuilder> _linkBuilders = new List<IHateoasLinkBuilder>();

        internal HateoasSource()
        {
        }

        public IEnumerable<IHateoasLinkBuilder> GetLinkBuilders()
        {
            return _linkBuilders;
        }

        public IHateoasLinkBuilder<T> AddLink(string routeName)
        {
            if (string.IsNullOrWhiteSpace(routeName))
            {
                throw new ArgumentNullException(nameof(routeName));
            }

            var linkBuilder = new HateoasLinkBuilder<T>(routeName);
            _linkBuilders.Add(linkBuilder);
            return linkBuilder;
        }
    }
}
