// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace HateoasNet.Tests.TestHelpers
{
    public class WhenDataAttribute : DataAttribute
    {
        /// <inheritdoc />
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var data = HateoasTestData.Valid(1, 1).Generate();

            yield return new object[] { data, new Func<HateoasTestData, bool>(x => !string.IsNullOrWhiteSpace(x.Prefix)) };
            yield return new object[] { data, new Func<HateoasTestData, bool>(x => !string.IsNullOrWhiteSpace(x.RouteName)) };
            yield return new object[] { data, new Func<HateoasTestData, bool>(x => !string.IsNullOrWhiteSpace(x.Template)) };
            yield return new object[] { data, new Func<HateoasTestData, bool>(x => x.ExpectedUrl != data.ExpectedUrl) };
        }
    }
}
