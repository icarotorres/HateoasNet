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
        private readonly IHateoasLinkBuilder<TestData> _sut;
        public HateoasLinkBuilderShould()
        {
            _sut = new HateoasLinkBuilder<TestData>("test");
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void HaveNotNullValues_For_RouteName_And_RouteDictionaryFunction_And_PredicateFunction()
        {
            _ = _sut.RouteName.Should().NotBeEmpty();
            _ = _sut.RouteDictionaryFunction.Should().NotBeNull();
            _ = _sut.Predicate.Should().NotBeNull();
        }

        [Fact]
        public void New_WithValidParameters_ReturnsHateoasLink()
        {
            _ = _sut.Should()
                .BeAssignableTo<HateoasLinkBuilder<TestData>>().And
                .BeAssignableTo<IHateoasLinkBuilder<TestData>>().And
                .BeOfType<HateoasLinkBuilder<TestData>>();
        }

        [Fact]
        public void HasRouteData_WithValidRouteDataFunction_ReturnIHateoasLink()
        {
            _ = _sut.HasRouteData(x => new { name = x.RouteName, controller = x.ControllerName });

            _ = _sut.RouteDictionaryFunction.Should().NotBeNull();
        }

        [Theory]
        [WhenData]
        public void IsApplicable_PredicateFunction_ReturnsSameValue(TestData data, Func<TestData, bool> function)
        {
            _ = _sut.When(function).IsSatisfiedBy(data).Should().Be(function(data));
        }

        [Fact]
        public void PresentedAs_PresentedName_ReturnsSameValue()
        {
            const string Expected = "new-name";
            _ = _sut.PresentedAs(Expected).PresentedName.Should().Be(Expected);
        }

        [Fact]
        public void GetRouteDictionary_HasRouteDataParameterFunction_ReturnsSameValue()
        {
            var data = new TestData();
            TestData.Faker.Populate(data, TestData.RuleSets.PrefixOnly);
            var linkBuilder = _sut.HasRouteData(x => new { name = x.RouteName, controller = x.ControllerName });
            var expected = new { name = data.RouteName, controller = data.ControllerName }.ToRouteDictionary();

            _sut.GetRouteDictionary(data).Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetRouteDictionary_WithResourceDataNull_ThrowsArgumentNullException()
        {
            _ = Assert.Throws<ArgumentNullException>(() => _sut.GetRouteDictionary(null));
        }

        [Fact]
        public void When_WithPredicateNull_ThrowsArgumentNullException()
        {
            _ = Assert.Throws<ArgumentNullException>(() => _sut.When(null));
        }

        [Fact]
        public void HasRouteData_WithRouteDataFunctionNull_ThrowsArgumentNullException()
        {
            _ = Assert.Throws<ArgumentNullException>(() => _sut.HasRouteData(null));
        }

        [Fact]
        public void IsSatisfiedBy_WithResourceDataNull_ThrowsArgumentNullException()
        {
            _ = Assert.Throws<ArgumentNullException>(() => _sut.IsSatisfiedBy(null));
        }
    }
}
