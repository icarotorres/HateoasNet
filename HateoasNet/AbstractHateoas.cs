﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace HateoasNet
{
    /// <inheritdoc cref="IHateoas{TOutput}" />
    public abstract class AbstractHateoas<TOutput> : IHateoas<TOutput>
    {
        private readonly IHateoas _hateoas;

        protected AbstractHateoas(IHateoas hateoas)
        {
            _hateoas = hateoas;
        }

        public TOutput Generate(object source)
        {
            return GenerateCustom(_hateoas.Generate(source));
        }

        /// <summary>
        ///   Transforms an available collection of <see cref="HateoasLink"/> into custom <typeparamref name="TOutput"/>.
        /// </summary>
        /// <param name="links">
        ///   Default generated collection of <see cref="HateoasLink"/> as source to generate customized <typeparamref name="TOutput"/>.
        /// </param>
        /// <returns>
        ///   A custom <typeparamref name="TOutput"/> created from <see cref="IEnumerable{T}" /> of <see cref="HateoasLink"/> ietms.
        /// </returns>
        protected abstract TOutput GenerateCustom(IEnumerable<HateoasLink> links);
    }
}
