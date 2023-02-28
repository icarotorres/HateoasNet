// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using HateoasNet.Abstractions;

namespace HateoasNet.Tests.TestHelpers
{
    public class TestDataBuilder : IHateoasSourceBuilder<Testdata>
    {
        public void Build(IHateoasSource<Testdata> source)
        {
            _ = source.AddLink("test1").When(x => !string.IsNullOrWhiteSpace(x.RouteName)).HasRouteData(x => new { route = x.RouteName });
            _ = source.AddLink("test2").When(x => !string.IsNullOrWhiteSpace(x.ControllerName)).HasRouteData(x => new { controller = x.ControllerName });
        }
    }
}
