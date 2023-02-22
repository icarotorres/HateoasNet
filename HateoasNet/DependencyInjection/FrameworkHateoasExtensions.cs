// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET472
namespace HateoasNet.DependencyInjection.Framework
{
    using System;
    using HateoasNet.Abstractions;
    using HateoasNet.Infrastructure;

    public static class HateoasExtensions
    {
        /// <summary>
        ///   Configure Hateoas Resource mapping in .Net Framework (Full) Web Api
        /// </summary>
        /// <param name="hateoasOptions"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IHateoasContext ConfigureHateoas(Func<IHateoasContext, IHateoasContext> hateoasOptions)
        {
            return hateoasOptions == null ? throw new ArgumentNullException(nameof(hateoasOptions)) : hateoasOptions(new HateoasContext());
        }
    }
}
#endif
