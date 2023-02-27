// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using FluentAssertions;
using HateoasNet.Abstractions;
using HateoasNet.Infrastructure;
using HateoasNet.Tests.TestHelpers;
using Xunit;

namespace HateoasNet.Tests.Infrastructure
{
    public class HateoasContextShould : IDisposable
    {
        private readonly TestData _data;
        private readonly IHateoasContext _sut;
        public HateoasContextShould()
        {
            _data = new TestData();
            TestData.Faker.Populate(_data, TestData.RuleSets.PrefixOnly);
            _sut = new HateoasContext();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void New_WithOutParameters_HateoasContext()
        {
            _ = _sut.Should().BeAssignableTo<IHateoasContext>().And.BeOfType<HateoasContext>();
        }

        [Fact]
        public void GetOrInsert_WithClassType_ReturnIHateoasSource()
        {
            // act
            var interfaceHateoasSource = _sut.GetOrInsert(typeof(TestData));
            var stronglyTypedHateoasSource = _sut.GetOrInsert<TestData>();

            // assert
            _ = stronglyTypedHateoasSource.Should().NotBeNull().And.BeSameAs(interfaceHateoasSource);
        }

        [Fact]
        public void GetApplicableLinks_WithNotConfiguredType_ReturnEmptyLinkBuilders()
        {
            _ = _sut.GetApplicableLinkBuilders(_data)
                .Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void GetApplicableLinks_WithConfiguredTypeAndValidCondition_ReturnLinkBuilders()
        {
            _ = _sut.Configure<TestData>(source =>
            {
                _ = source.AddLink("test")
                        .When(x => !string.IsNullOrEmpty(x.ControllerName))
                        .HasRouteData(x => new { controller = x.ControllerName });
            }).GetApplicableLinkBuilders(_data)
              .Should().NotBeNull().And.NotBeEmpty();
        }

        [Fact]
        public void GetApplicableLinks_WithConfiguredTypeAndInvalidCondition_ReturnEmptyLinkBuilders()
        {
            var routeNameNullData = new TestData();
            TestData.Faker.Populate(routeNameNullData, TestData.RuleSets.RouteNameNull);
            _ = _sut.Configure<TestData>(source =>
            {
                _ = source.AddLink("test")
                        .When(x => !string.IsNullOrEmpty(x.RouteName))
                        .HasRouteData(x => new { route = x.RouteName });
            }).GetApplicableLinkBuilders(routeNameNullData)
              .Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void GetApplicableLinks_WithApplyConfigurationForType_ReturnLinkBuilders()
        {
            _ = _sut.ApplyConfiguration(new TestDataBuilder())
                .GetApplicableLinkBuilders(_data)
                .Should().NotBeNull().And.NotBeEmpty();
        }

        [Fact]
        public void GetApplicableLinks_WithTypeConfiguredFromAssembly_ReturnLinkBuilders()
        {
            _ = _sut.ConfigureFromAssembly(typeof(TestDataBuilder).Assembly)
                .GetApplicableLinkBuilders(_data)
                .Should().NotBeNull().And.NotBeEmpty();
        }

        [Fact]
        public void ApplyConfiguration_WithResourceNull_ThrowsArgumentNullException()
        {
            _ = Assert.Throws<ArgumentNullException>(() => _sut.ApplyConfiguration<TestData>(null));
        }

        [Fact]
        public void Configure_WithResourceNull_ThrowsArgumentNullException()
        {
            _ = Assert.Throws<ArgumentNullException>(() => _sut.Configure<TestData>(null));
        }

        [Fact]
        public void ConfigureFromAssembly_WhenHasNo_IHateoasSourceConfiguration_ThrowsTargetException()
        {
            _ = Assert.Throws<TargetException>(() => _sut.ConfigureFromAssembly(typeof(string).Assembly));
        }

        [Fact]
        public void ConfigureFromAssembly_WithResourceNull_ThrowsArgumentNullException()
        {
            _ = Assert.Throws<ArgumentNullException>(() => _sut.ConfigureFromAssembly(null));
        }
    }
}
