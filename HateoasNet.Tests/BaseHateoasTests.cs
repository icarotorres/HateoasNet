// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using HateoasNet.Abstractions;
using HateoasNet.Tests.TestHelpers;
using Moq;
using Xunit;

namespace HateoasNet.Tests
{
    public abstract class BaseHateoasTests : IDisposable
    {
        protected IHateoas _sut;
        protected readonly Testdata _data;
        protected readonly Mock<IHateoasContext> _mockHateoasContext;
        protected BaseHateoasTests()
        {
            _data = new Testdata();
            _mockHateoasContext = new Mock<IHateoasContext>().SetupAllProperties();
        }
        protected abstract void MockFor_NullRouteName_ThrowsArgumentNullException();
        protected abstract void MockFor_ValidParameters_ReturnsHateoasLinks();
        /// <inheritdoc />
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void Generate_NullRouteName_ThrowsArgumentNullException()
        {
            // arrange
            Fakers.Testdata.Populate(_data, Fakers.RuleSets.RouteNameNull);
            MockHateoasContext();
            MockFor_NullRouteName_ThrowsArgumentNullException();

            // act & assert
            _ = Assert.Throws<ArgumentNullException>(() => _sut.Generate(_data.RouteValues));
        }

        [Theory]
        [InlineData(Fakers.RuleSets.PrefixOnly)]
        [InlineData(Fakers.RuleSets.RouteTemplate)]
        [InlineData(Fakers.RuleSets.QueryStrings)]
        [InlineData(Fakers.RuleSets.Complete)]
        public void Generate_ValidParameters_ReturnsHateoasLinks(string rulesets)
        {
            // arrange
            Fakers.Testdata.Populate(_data, rulesets);
            var expected = new HateoasLink(_data.RouteName, _data.ExpectedUrl, _data.Method.Method);
            MockHateoasContext();
            MockFor_ValidParameters_ReturnsHateoasLinks();

            // act
            var links = _sut.Generate(_data.RouteValues);

            // assert
            _ = links.Should()
                .NotBeEmpty().And
                .BeAssignableTo<IEnumerable<HateoasLink>>().And
                .BeEquivalentTo(new HateoasLink[] { expected });
        }

        protected void MockHateoasContext()
        {
            var mockLinkBuilder = new Mock<IHateoasLinkBuilder>();
            _ = mockLinkBuilder.Setup(x => x.RouteName).Returns(_data.RouteName);
            _ = mockLinkBuilder.Setup(x => x.PresentedName).Returns(_data.RouteName);
            _ = mockLinkBuilder.Setup(x => x.GetRouteDictionary(It.IsAny<object>())).Returns(_data.RouteValues);

            _ = _mockHateoasContext
                .Setup(x => x.GetApplicableLinkBuilders(_data.RouteValues))
                .Returns(new IHateoasLinkBuilder[] { mockLinkBuilder.Object });
        }
    }
}
