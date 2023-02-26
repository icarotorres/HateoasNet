// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using HateoasNet.Abstractions;
using HateoasNet.Tests.TestHelpers;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using FluentAssertions;
#if NET7_0 || NETCOREAPP3_1
using HateoasNet.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
#elif NET472 || NET48
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Routing;
using System.Text;

#endif
namespace HateoasNet.Tests
{
    public class HateoasShould : IDisposable
    {
        private Hateoas _sut;
        private readonly Mock<IHateoasContext> _mockHateoasContext;

        /// <inheritdoc />
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void Generate_NullRouteName_ThrowsArgumentNullException()
        {
            // arrange
            var mockData = HateoasTestData.Valid(0, 0).Generate();
            mockData.RouteName = null;
            MockHateoasContext(mockData);

#if NET7_0 || NETCOREAPP3_1
            MockEmptyActionDescriptor();
            _sut = new Hateoas(_mockHateoasContext.Object, _mockUrlHelper.Object, _mockActionDescriptorCollectionProvider.Object);
#elif NET472 || NET48
            _sut = new Hateoas(_mockHateoasContext.Object);
#endif

            // act & assert
            _ = Assert.Throws<ArgumentNullException>(() => _sut.Generate(mockData.RouteValues));
        }

        [Theory]
        [InlineData(3, 0)]
        [InlineData(2, 2)]
        [InlineData(0, 3)]
        public void Generate_ValidParameters_ReturnsHateoasLinks(int parametersCount, int queryStringsCount)
        {
            // arrange
            var mockData = HateoasTestData.Valid(parametersCount, queryStringsCount).Generate();
            var expected = new HateoasLink(mockData.RouteName, mockData.ExpectedUrl, mockData.Method.Method);
            MockHateoasContext(mockData);

#if NET7_0 || NETCOREAPP3_1
            _ = _mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(mockData.ExpectedUrl);
            MockActionDescriptor(mockData);
            _sut = new Hateoas(_mockHateoasContext.Object, _mockUrlHelper.Object, _mockActionDescriptorCollectionProvider.Object);
#elif NET472 || NET48
            MockHttpContextAndActionDescriptors(mockData);
            _sut = new Hateoas(_mockHateoasContext.Object);
#endif

            // act
            var links = _sut.Generate(mockData.RouteValues);

            // assert
            _ = links.Should()
                .NotBeEmpty().And
                .BeAssignableTo<IEnumerable<HateoasLink>>().And
                .BeEquivalentTo(new HateoasLink[] { expected });
        }

        private void MockHateoasContext(HateoasTestData mockData)
        {
            var mockLinkBuilder = new Mock<IHateoasLinkBuilder>();
            _ = mockLinkBuilder.Setup(x => x.RouteName).Returns(mockData.RouteName);
            _ = mockLinkBuilder.Setup(x => x.PresentedName).Returns(mockData.RouteName);
            _ = mockLinkBuilder.Setup(x => x.GetRouteDictionary(It.IsAny<object>())).Returns(mockData.RouteValues);

            _ = _mockHateoasContext
                .Setup(x => x.GetApplicableLinkBuilders(mockData.RouteValues))
                .Returns(new IHateoasLinkBuilder[] { mockLinkBuilder.Object });
        }

#if NET7_0 || NETCOREAPP3_1
        private readonly Mock<IUrlHelper> _mockUrlHelper;
        private readonly Mock<IActionDescriptorCollectionProvider> _mockActionDescriptorCollectionProvider;

        public HateoasShould()
        {
            _mockHateoasContext = new Mock<IHateoasContext>().SetupAllProperties();
            _mockActionDescriptorCollectionProvider = new Mock<IActionDescriptorCollectionProvider>().SetupAllProperties();
            _mockUrlHelper = new Mock<IUrlHelper>().SetupAllProperties();
        }

        [Fact]
        public void Generate_ActionDescriptorNotFound_ThrowsArgumentNullException()
        {
            // arrange
            var mockData = HateoasTestData.Valid(0, 0).Generate();
            MockHateoasContext(mockData);
            MockEmptyActionDescriptor();
            _sut = new Hateoas(_mockHateoasContext.Object, _mockUrlHelper.Object, _mockActionDescriptorCollectionProvider.Object);

            // act & assert
            _ = Assert.Throws<NotSupportedException>(() => _sut.Generate(mockData.RouteValues));
        }

        private void MockActionDescriptor(HateoasTestData mockData)
        {
            var actionDescriptors = new List<ActionDescriptor>
            {
                new ActionDescriptor
                {
                    AttributeRouteInfo = new AttributeRouteInfo {Name = mockData.RouteName},
                    ActionConstraints = new List<IActionConstraintMetadata>
                    {
                        new HttpMethodActionConstraint(new[] {mockData.Method.Method})
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

#elif NET472 || NET48
        public HateoasShould()
        {
            _mockHateoasContext = new Mock<IHateoasContext>().SetupAllProperties();
        }

        [Fact]
        public void Generate_HttpActionDescriptorNull_ThrowsArgumentNullException()
        {
            // arrange
            var mockData = HateoasTestData.Valid(1, 1).Generate();
            MockHateoasContext(mockData);
            // not mocking ActionDescriptor resulting on it being null
            _sut = new Hateoas(_mockHateoasContext.Object);

            // act & assert
            _ = Assert.Throws<NotSupportedException>(() => _sut.Generate(mockData.RouteValues));
        }

        [Fact]
        public void Generate_InvalidParameterNotInRouteValues_ThrowsException()
        {
            // arrange
            var mockData = HateoasTestData.InvalidParameterNotInRouteValues("qweasdzxc").Generate();
            MockHateoasContext(mockData);
            MockHttpContextAndActionDescriptors(mockData);
            _sut = new Hateoas(_mockHateoasContext.Object);

            // act & assert
            _ = Assert.Throws<InvalidOperationException>(() => _sut.Generate(mockData.RouteValues));
        }

        private void MockHttpContextAndActionDescriptors(HateoasTestData mockData)
        {
            var config = new Mock<HttpConfiguration>().Object;
            var prefix = new RoutePrefixAttribute(mockData.Prefix);
            var controllerDescriptor = new TestControllerDescriptor(config, mockData.ControllerName, typeof(ApiController), prefix);
            var actionParameters =
                mockData.QueryStrings.Aggregate(new Collection<HttpParameterDescriptor>(),
                (collection, parameterName) =>
                {
                    var mockParameterDescriptor = new Mock<HttpParameterDescriptor>();
                    _ = mockParameterDescriptor.Setup(x => x.ParameterName).Returns(parameterName);
                    collection.Add(mockParameterDescriptor.Object);
                    return collection;
                });
            var methodInfo = new TestMethodInfo(new RouteAttribute(mockData.Template) { Name = mockData.RouteName });
            var actionDescriptor = new TestActionDescriptor(controllerDescriptor, methodInfo, actionParameters);
            actionDescriptor.SupportedHttpMethods.Add(mockData.Method);
            var dataTokens = new RouteValueDictionary
            {
                { "descriptors", new[] { actionDescriptor } }
            };

            RouteTable.Routes.Add(new Route(mockData.RoutePath, null) { DataTokens = dataTokens });
            var request = new HttpRequest("", mockData.BaseUrl, "");
            var response = new HttpResponse(new StringWriter());
            HttpContext.Current = new HttpContext(request, response);
        }
#endif
    }
}
