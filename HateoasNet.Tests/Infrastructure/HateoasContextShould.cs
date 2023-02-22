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
        [Trait(nameof(IHateoasContext), "New")]
        public void New_WithOutParameters_HateoasContext()
        {
            _ = _sut.Should().BeAssignableTo<IHateoasContext>().And.BeOfType<HateoasContext>();
        }

        [Fact]
        [Trait(nameof(IHateoasContext), nameof(HateoasContext.GetOrInsert))]
        public void GetOrInsert_WithClassType_ReturnIHateoasSource()
        {
            // act
            var interfaceHateoasSource = (_sut as HateoasContext)?.GetOrInsert(typeof(HateoasSample));
            var stronglyTypedHateoasSource = (_sut as HateoasContext)?.GetOrInsert<HateoasSample>();

            // assert
            _ = stronglyTypedHateoasSource.Should().NotBeNull().And.BeSameAs(interfaceHateoasSource);
        }

        [Fact]
        [Trait(nameof(IHateoasContext), nameof(IHateoasContext.GetApplicableLinkBuilders))]
        public void GetApplicableLinks_WithNotConfiguredType_ReturnEmptyLinkBuilders()
        {
            _ = _sut.GetApplicableLinkBuilders(new HateoasSample())
                .Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        [Trait(nameof(IHateoasContext), nameof(IHateoasContext.Configure))]
        [Trait(nameof(IHateoasContext), nameof(IHateoasContext.GetApplicableLinkBuilders))]
        public void GetApplicableLinks_WithConfiguredType_ReturnLinkBuilders()
        {
            _ = _sut.Configure<HateoasSample>(source =>
            {
                _ = source.AddLink("test")
                        .When(x => x.Id != null && x.Id != Guid.Empty)
                        .HasRouteData(x => new { id = x.Id });
            }).GetApplicableLinkBuilders(new HateoasSample())
              .Should().NotBeNull().And.NotBeEmpty();
        }

        [Fact]
        [Trait(nameof(IHateoasContext), nameof(IHateoasContext.ApplyConfiguration))]
        [Trait(nameof(IHateoasContext), nameof(IHateoasContext.GetApplicableLinkBuilders))]
        public void GetApplicableLinks_WithApplyConfigurationForType_ReturnLinkBuilders()
        {
            _ = _sut.ApplyConfiguration(new HateoasSampleBuilder())
                .GetApplicableLinkBuilders(new HateoasSample())
                .Should().NotBeNull().And.NotBeEmpty();
        }

        [Fact]
        [Trait(nameof(IHateoasContext), nameof(IHateoasContext.ConfigureFromAssembly))]
        [Trait(nameof(IHateoasContext), nameof(IHateoasContext.GetApplicableLinkBuilders))]
        public void GetApplicableLinks_WithTypeConfiguredFromAssembly_ReturnLinkBuilders()
        {
            _ = _sut.ConfigureFromAssembly(new HateoasSampleBuilder().GetType().Assembly)
                .GetApplicableLinkBuilders(new HateoasSample())
                .Should().NotBeNull().And.NotBeEmpty();
        }

        [Fact]
        [Trait(nameof(IHateoasContext), nameof(IHateoasContext.ApplyConfiguration))]
        [Trait(nameof(IHateoasContext), "Exceptions")]
        public void ApplyConfiguration_WithResourceNull_ThrowsArgumentNullException()
        {
            // arrange
            const string parameterName = "configuration";

            // act
            void actual()
            {
                _ = _sut.ApplyConfiguration<HateoasSample>(null);
            }

            _ = Assert.Throws<ArgumentNullException>(parameterName, actual);
        }

        [Fact]
        [Trait(nameof(IHateoasContext), nameof(IHateoasContext.Configure))]
        [Trait(nameof(IHateoasContext), "Exceptions")]
        public void Configure_WithResourceNull_ThrowsArgumentNullException()
        {
            // arrange
            const string parameterName = "source";

            // act
            void actual()
            {
                _ = _sut.Configure<HateoasSample>(null);
            }

            // assert
            _ = Assert.Throws<ArgumentNullException>(parameterName, actual);
        }

        [Fact]
        [Trait(nameof(IHateoasContext), nameof(IHateoasContext.ConfigureFromAssembly))]
        [Trait(nameof(IHateoasContext), "Exceptions")]
        public void ConfigureFromAssembly_WhenHasNo_IHateoasSourceConfiguration_ThrowsTargetException()
        {
            // act
            void actual()
            {
                _ = _sut.ConfigureFromAssembly(typeof(string).Assembly);
            }

            // assert
            _ = Assert.Throws<TargetException>(actual);
        }

        [Fact]
        [Trait(nameof(IHateoasContext), nameof(IHateoasContext.ConfigureFromAssembly))]
        [Trait(nameof(IHateoasContext), "Exceptions")]
        public void ConfigureFromAssembly_WithResourceNull_ThrowsArgumentNullException()
        {
            // arrange
            const string parameterName = "assembly";

            // act
            void actual()
            {
                _ = _sut.ConfigureFromAssembly(null);
            }

            // assert
            _ = Assert.Throws<ArgumentNullException>(parameterName, actual);
        }
    }
}
