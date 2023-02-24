// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using FluentAssertions;
using HateoasNet.Abstractions;
using HateoasNet.Infrastructure;
using Xunit;

namespace HateoasNet.Tests.Infrastructure
{
    public class HateoasContextShould : IDisposable
    {
        private readonly IHateoasContext _sut;
        public HateoasContextShould()
        {
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
            var interfaceHateoasSource = _sut.GetOrInsert(typeof(HateoasSample));
            var stronglyTypedHateoasSource = _sut.GetOrInsert<HateoasSample>();

            // assert
            _ = stronglyTypedHateoasSource.Should().NotBeNull().And.BeSameAs(interfaceHateoasSource);
        }

        [Fact]
        public void GetApplicableLinks_WithNotConfiguredType_ReturnEmptyLinkBuilders()
        {
            _ = _sut.GetApplicableLinkBuilders(new HateoasSample())
                .Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void GetApplicableLinks_WithConfiguredType_ReturnLinkBuilders()
        {
            _ = _sut.Configure<HateoasSample>(source =>
            {
                _ = source.AddLink("test")
                        .When(x => x.Id != Guid.Empty)
                        .HasRouteData(x => new { id = x.Id });
            }).GetApplicableLinkBuilders(new HateoasSample())
              .Should().NotBeNull().And.NotBeEmpty();
        }

        [Fact]
        public void GetApplicableLinks_WithApplyConfigurationForType_ReturnLinkBuilders()
        {
            _ = _sut.ApplyConfiguration(new HateoasSampleBuilder())
                .GetApplicableLinkBuilders(new HateoasSample())
                .Should().NotBeNull().And.NotBeEmpty();
        }

        [Fact]
        public void GetApplicableLinks_WithTypeConfiguredFromAssembly_ReturnLinkBuilders()
        {
            _ = _sut.ConfigureFromAssembly(new HateoasSampleBuilder().GetType().Assembly)
                .GetApplicableLinkBuilders(new HateoasSample())
                .Should().NotBeNull().And.NotBeEmpty();
        }

        [Fact]
        public void ApplyConfiguration_WithResourceNull_ThrowsArgumentNullException()
        {
            _ = Assert.Throws<ArgumentNullException>(() =>_sut.ApplyConfiguration<HateoasSample>(null));
        }

        [Fact]
        public void Configure_WithResourceNull_ThrowsArgumentNullException()
        {
            _ = Assert.Throws<ArgumentNullException>(() => _sut.Configure<HateoasSample>(null));
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
