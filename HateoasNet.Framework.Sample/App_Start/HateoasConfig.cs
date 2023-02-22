// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using HateoasNet.Abstractions;
using HateoasNet.DependencyInjection.Framework;

namespace HateoasNet.Framework.Sample
{
    public static class HateoasConfig
    {
        public static IHateoasContext ConfigureFromAssembly(Type type)
        {
            return HateoasExtensions.ConfigureHateoas(context => context.ConfigureFromAssembly(type.Assembly));
        }
    }
}
