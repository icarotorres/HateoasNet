// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using HateoasNet.Abstractions;
using HateoasNet.Infrastructure;
using Xunit;

namespace HateoasNet.Tests.Infrastructure
{
    public class HateoasSourceShould : IDisposable
    {
        private readonly HateoasSource<HateoasSample> _sut;

        public HateoasSourceShould()
        {
            _sut = new HateoasSource<HateoasSample>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        [Fact]
        [Trait(nameof(IHateoasSource), "Instance")]
        public void New_WithTypeParameter_CreatesHateoasSource()
        {
            _ = _sut.Should()
                .BeAssignableTo<IHateoasSource>().And
                .BeAssignableTo<IHateoasSource<HateoasSample>>().And
                .BeOfType<HateoasSource<HateoasSample>>();
        }

        [Fact]
        [Trait(nameof(IHateoasSource), nameof(IHateoasSource.GetLinkBuilders))]
        public void GetLinks_FromHateoasSource_WithOutConfiguredLinks_ReturnsEmptyLinkBuilders()
        {
            _ = _sut.GetLinkBuilders().Should().BeAssignableTo<IEnumerable<IHateoasLinkBuilder>>().And.BeEmpty();
        }

        [Fact]
        [Trait(nameof(IHateoasSource), nameof(IHateoasSource<HateoasSample>.AddLink))]
        public void HasLink_WithNotEmptyString_ReturnsHateoasLinkBuilder()
        {
            _ = _sut.AddLink("not empty string").Should()
                .NotBeNull().And
                .BeAssignableTo<IHateoasLinkBuilder>().And
                .BeAssignableTo<IHateoasLinkBuilder<HateoasSample>>().And
                .BeOfType<HateoasLinkBuilder<HateoasSample>>();
        }

        [Fact]
        [Trait(nameof(IHateoasSource), nameof(IHateoasSource.GetLinkBuilders))]
        public void GetLinks_FromHateoasSource_WithConfiguredLinks_ReturnsLinkBuilders()
        {
            // arrange
            var linkBuilder = _sut.AddLink("not empty string").Should()
                .BeAssignableTo<IHateoasLinkBuilder>().And
                .BeAssignableTo<IHateoasLinkBuilder<HateoasSample>>().And
                .BeOfType<HateoasLinkBuilder<HateoasSample>>().Subject;

            _ = _sut.GetLinkBuilders().Should()
                .BeAssignableTo<IEnumerable<IHateoasLinkBuilder>>().And
                .Contain(linkBuilder);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        [Trait(nameof(IHateoasSource), nameof(IHateoasSource<HateoasSample>.AddLink))]
        [Trait(nameof(IHateoasSource), "Exceptions")]
        public void HasLink_WithRouteNameNullOrEmpty_ThrowsArgumentNullException(string routeName)
        {
            // arrange
            const string parameterName = "routeName";

            // act
            void actual()
            {
                _ = _sut.AddLink(routeName);
            }

            _ = Assert.Throws<ArgumentNullException>(parameterName, actual);
        }
    }
}
