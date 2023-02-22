// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FluentAssertions;
using HateoasNet.Abstractions;
using HateoasNet.Extensions;
using HateoasNet.Infrastructure;
using HateoasNet.Tests.TestHelpers;
using Xunit;

namespace HateoasNet.Tests.Infrastructure
{
    public class HateoasLinkBuilderShould : IDisposable
    {
        private readonly HateoasLinkBuilder<HateoasSample> _sut;
        public HateoasLinkBuilderShould()
        {
            _sut = new HateoasLinkBuilder<HateoasSample>("test");
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        [Fact]
        [Trait(nameof(IHateoasLinkBuilder), "Instance")]
        public void HaveNotNullValues_For_RouteName_And_RouteDictionaryFunction_And_PredicateFunction()
        {
            _ = _sut.RouteName.Should().NotBeEmpty();
            _ = _sut.RouteDictionaryFunction.Should().NotBeNull();
            _ = _sut.Predicate.Should().NotBeNull();
        }

        [Fact]
        [Trait(nameof(IHateoasLinkBuilder), "Instance")]
        public void New_WithValidParameters_ReturnsHateoasLink()
        {
            _ = _sut.Should()
                .BeAssignableTo<IHateoasLinkBuilder>().And
                .BeAssignableTo<IHateoasLinkBuilder<HateoasSample>>().And
                .BeOfType<HateoasLinkBuilder<HateoasSample>>();
        }

        [Fact]
        [Trait(nameof(IHateoasLinkBuilder), nameof(IHateoasLinkBuilder<HateoasSample>.HasRouteData))]
        [Trait(nameof(IHateoasLinkBuilder), nameof(IHateoasLinkBuilder<HateoasSample>.RouteDictionaryFunction))]
        public void HasRouteData_WithValidRouteDataFunction_ReturnIHateoasLink()
        {
            _ = _sut.HasRouteData(x => new { id = x.Id, foreignKey = x.ForeignKeyId });

            _ = _sut.RouteDictionaryFunction.Should().NotBeNull();
        }

        [Theory]
        [WhenData]
        [Trait(nameof(IHateoasLinkBuilder), nameof(IHateoasLinkBuilder.IsApplicable))]
        [Trait(nameof(IHateoasLinkBuilder), nameof(IHateoasLinkBuilder<HateoasSample>.When))]
        public void IsApplicable_PredicateFunction_ReturnsSameValue(HateoasSample data, Func<HateoasSample, bool> function)
        {
            _ = _sut.When(function).IsApplicable(data).Should().Be(function(data));
        }

        [Fact]
        [Trait(nameof(IHateoasLinkBuilder), nameof(IHateoasLinkBuilder<HateoasSample>.PresentedAs))]
        public void PresentedAs_PresentedName_ReturnsSameValue()
        {
            const string expected = "new-name";
            _ = _sut.PresentedAs(expected).PresentedName.Should().Be(expected);
        }

        [Fact]
        [Trait(nameof(IHateoasLinkBuilder), nameof(IHateoasLinkBuilder<HateoasSample>.GetRouteDictionary))]
        [Trait(nameof(IHateoasLinkBuilder), nameof(IHateoasLinkBuilder<HateoasSample>.HasRouteData))]
        public void GetRouteDictionary_HasRouteDataParameterFunction_ReturnsSameValue()
        {
            var data = new HateoasSample();
            var linkBuilder = _sut.HasRouteData(x => new { id = x.Id, foreignKey = x.ForeignKeyId });
            var expected = new { id = data.Id, foreignKey = data.ForeignKeyId }.ToRouteDictionary();

            _sut.GetRouteDictionary(data).Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait(nameof(IHateoasLinkBuilder), nameof(IHateoasLinkBuilder<HateoasSample>.GetRouteDictionary))]
        [Trait(nameof(IHateoasLinkBuilder), "Exceptions")]
        public void GetRouteDictionary_WithResourceDataNull_ThrowsArgumentNullException()
        {
            // arrange
            const string parameterName = "source";

            // act
            void actual()
            {
                _ = _sut.GetRouteDictionary(null);
            }

            // assert
            _ = Assert.Throws<ArgumentNullException>(parameterName, actual);
        }

        [Fact]
        [Trait(nameof(IHateoasLinkBuilder), nameof(IHateoasLinkBuilder<HateoasSample>.When))]
        [Trait(nameof(IHateoasLinkBuilder), "Exceptions")]
        public void HasConditional_WithPredicateNull_ThrowsArgumentNullException()
        {
            // arrange
            const string parameterName = "predicate";

            // act
            void actual()
            {
                _ = _sut.When(null);
            }

            // assert
            _ = Assert.Throws<ArgumentNullException>(parameterName, actual);
        }

        [Fact]
        [Trait(nameof(IHateoasLinkBuilder), nameof(IHateoasLinkBuilder<HateoasSample>.HasRouteData))]
        [Trait(nameof(IHateoasLinkBuilder), "Exceptions")]
        public void HasRouteData_WithRouteDataFunctionNull_ThrowsArgumentNullException()
        {
            // arrange
            const string parameterName = "routeDataFunction";

            // act
            void actual()
            {
                _ = _sut.HasRouteData(null);
            }

            // assert
            _ = Assert.Throws<ArgumentNullException>(parameterName, actual);
        }

        [Fact]
        [Trait(nameof(IHateoasLinkBuilder), nameof(IHateoasLinkBuilder.IsApplicable))]
        [Trait(nameof(IHateoasLinkBuilder), "Exceptions")]
        public void IsApplicable_WithResourceDataNull_ThrowsArgumentNullException()
        {
            // arrange
            const string parameterName = "source";

            // act
            void actual()
            {
                _ = _sut.IsApplicable(null);
            }

            // assert
            _ = Assert.Throws<ArgumentNullException>(parameterName, actual);
        }
    }
}
