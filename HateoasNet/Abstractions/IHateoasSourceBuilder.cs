// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace HateoasNet.Abstractions
{
    /// <summary>
    ///   Represents configuration in separated class for <see cref="IHateoasSource{T}"/> targeting <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHateoasSourceBuilder<T> where T : class
    {
        /// <summary>
        ///   Configure <see cref="IHateoasSource{T}"/> using implemented logic.
        /// </summary>
        /// <param name="source">An <see cref="IHateoasSource{T}" /> instance to configure, generated internally.</param>
        void Build(IHateoasSource<T> source);
    }
}
