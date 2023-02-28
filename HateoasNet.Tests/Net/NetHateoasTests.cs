// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET7_0 || NETCOREAPP3_1
using HateoasNet.Tests.TestHelpers;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace HateoasNet.Tests.Net
{
    public class NetHateoasTests : BaseHateoasTests
    {
        private readonly Mock<IUrlHelper> _mockUrlHelper;
        private readonly Mock<IActionDescriptorCollectionProvider> _mockActionDescriptorCollectionProvider;

        public NetHateoasTests()
        {
            _mockActionDescriptorCollectionProvider = new Mock<IActionDescriptorCollectionProvider>().SetupAllProperties();
            _mockUrlHelper = new Mock<IUrlHelper>().SetupAllProperties();
        }

        protected override void MockFor_NullRouteName_ThrowsArgumentNullException()
        {
            MockEmptyActionDescriptor();
            _sut = new Hateoas(_mockHateoasContext.Object, _mockUrlHelper.Object, _mockActionDescriptorCollectionProvider.Object);
        }

        protected override void MockFor_ValidParameters_ReturnsHateoasLinks()
        {
            _ = _mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(_data.ExpectedUrl);
            MockActionDescriptor();
            _sut = new Hateoas(_mockHateoasContext.Object, _mockUrlHelper.Object, _mockActionDescriptorCollectionProvider.Object);
        }

        [Fact]
        public void Generate_ActionDescriptorNotFound_ThrowsArgumentNullException()
        {
            // arrange
            Fakers.Testdata.Populate(_data, Fakers.RuleSets.PrefixOnly);
            MockHateoasContext();
            MockEmptyActionDescriptor();
            _sut = new Hateoas(_mockHateoasContext.Object, _mockUrlHelper.Object, _mockActionDescriptorCollectionProvider.Object);

            // act & assert
            _ = Assert.Throws<NotSupportedException>(() => _sut.Generate(_data.RouteValues));
        }

        private void MockActionDescriptor()
        {
            var actionDescriptors = new List<ActionDescriptor>
            {
                new ActionDescriptor
                {
                    AttributeRouteInfo = new AttributeRouteInfo {Name = _data.RouteName},
                    ActionConstraints = new List<IActionConstraintMetadata>
                    {
                        new HttpMethodActionConstraint(new[] {_data.Method.Method})
                    }
                }
            };
            _ = _mockActionDescriptorCollectionProvider
                .Setup(x => x.ActionDescriptors)
                .Returns(new ActionDescriptorCollection(actionDescriptors, 1));
        }

        private void MockEmptyActionDescriptor()
        {
            _ = _mockActionDescriptorCollectionProvider
                .Setup(x => x.ActionDescriptors)
                .Returns(new ActionDescriptorCollection(Array.Empty<ActionDescriptor>(), 1));
        }
    }
}
#endif
