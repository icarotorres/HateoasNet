// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET472 || NET48
using System;
using HateoasNet.Tests.TestHelpers;
using Xunit;

namespace HateoasNet.Tests.NetFramework
{
    public class NetFrameworkGenericHateoasTests : BaseGenericHateoasTests
    {
        protected override void MockFor_NullRouteName_ThrowsArgumentNullException()
        {
            _sut = new DictionaryHateoas(new Hateoas(_mockHateoasContext.Object));
        }

        protected override void MockFor_ValidParameters_ReturnsHateoasLinks()
        {
            NetFrameworkStaticMocks.MockHttpContextAndActionDescriptors(_data);
            _sut = new DictionaryHateoas(new Hateoas(_mockHateoasContext.Object));
        }

        [Fact]
        public void Generate_HttpActionDescriptorNull_ThrowsArgumentNullException()
        {
            // arrange
            Fakers.Testdata.Populate(_data, Fakers.RuleSets.PrefixOnly);
            MockHateoasContext();
            // not mocking ActionDescriptor resulting on it being null
            _sut = new DictionaryHateoas(new Hateoas(_mockHateoasContext.Object));

            // act & assert
            _ = Assert.Throws<NotSupportedException>(() => _sut.Generate(_data.RouteValues));
        }

        [Fact]
        public void Generate_InvalidParameterNotInRouteValues_ThrowsException()
        {
            // arrange
            Fakers.Testdata.Populate(_data, Fakers.RuleSets.ParameterNotInRouteValues);
            MockHateoasContext();
            NetFrameworkStaticMocks.MockHttpContextAndActionDescriptors(_data);
            _sut = new DictionaryHateoas(new Hateoas(_mockHateoasContext.Object));

            // act & assert
            _ = Assert.Throws<InvalidOperationException>(() => _sut.Generate(_data.RouteValues));
        }
    }
}
#endif
