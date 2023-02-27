// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using HateoasNet.Abstractions;
using HateoasNet.Tests.TestHelpers;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
#if NET7_0 || NETCOREAPP3_1
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
#elif NET472 || NET48
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Routing;

#endif
namespace HateoasNet.Tests
{
    public class HateoasShould : IDisposable
    {
        private Hateoas _sut;
        private readonly TestData _data;
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
            TestData.Faker.Populate(_data, TestData.RuleSets.RouteNameNull);
            MockHateoasContext();

#if NET7_0 || NETCOREAPP3_1
            MockEmptyActionDescriptor();
            _sut = new Hateoas(_mockHateoasContext.Object, _mockUrlHelper.Object, _mockActionDescriptorCollectionProvider.Object);
#elif NET472 || NET48
            _sut = new Hateoas(_mockHateoasContext.Object);
#endif

            // act & assert
            _ = Assert.Throws<ArgumentNullException>(() => _sut.Generate(_data.RouteValues));
        }

        [Theory]
        [InlineData(TestData.RuleSets.PrefixOnly)]
        [InlineData(TestData.RuleSets.RouteTemplate)]
        [InlineData(TestData.RuleSets.QueryStrings)]
        [InlineData(TestData.RuleSets.Complete)]
        public void Generate_ValidParameters_ReturnsHateoasLinks(string rulesets)
        {
            // arrange
            TestData.Faker.Populate(_data, rulesets);
            var expected = new HateoasLink(_data.RouteName, _data.ExpectedUrl, _data.Method.Method);
            MockHateoasContext();

#if NET7_0 || NETCOREAPP3_1
            _ = _mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(_data.ExpectedUrl);
            MockActionDescriptor();
            _sut = new Hateoas(_mockHateoasContext.Object, _mockUrlHelper.Object, _mockActionDescriptorCollectionProvider.Object);
#elif NET472 || NET48
            MockHttpContextAndActionDescriptors();
            _sut = new Hateoas(_mockHateoasContext.Object);
#endif

            // act
            var links = _sut.Generate(_data.RouteValues);

            // assert
            _ = links.Should()
                .NotBeEmpty().And
                .BeAssignableTo<IEnumerable<HateoasLink>>().And
                .BeEquivalentTo(new HateoasLink[] { expected });
        }

        private void MockHateoasContext()
        {
            var mockLinkBuilder = new Mock<IHateoasLinkBuilder>();
            _ = mockLinkBuilder.Setup(x => x.RouteName).Returns(_data.RouteName);
            _ = mockLinkBuilder.Setup(x => x.PresentedName).Returns(_data.RouteName);
            _ = mockLinkBuilder.Setup(x => x.GetRouteDictionary(It.IsAny<object>())).Returns(_data.RouteValues);

            _ = _mockHateoasContext
                .Setup(x => x.GetApplicableLinkBuilders(_data.RouteValues))
                .Returns(new IHateoasLinkBuilder[] { mockLinkBuilder.Object });
        }

#if NET7_0 || NETCOREAPP3_1
        private readonly Mock<IUrlHelper> _mockUrlHelper;
        private readonly Mock<IActionDescriptorCollectionProvider> _mockActionDescriptorCollectionProvider;

        public HateoasShould()
        {
            _data = new TestData();
            _mockHateoasContext = new Mock<IHateoasContext>().SetupAllProperties();
            _mockActionDescriptorCollectionProvider = new Mock<IActionDescriptorCollectionProvider>().SetupAllProperties();
            _mockUrlHelper = new Mock<IUrlHelper>().SetupAllProperties();
        }

        [Fact]
        public void Generate_ActionDescriptorNotFound_ThrowsArgumentNullException()
        {
            // arrange
            TestData.Faker.Populate(_data, TestData.RuleSets.PrefixOnly);
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

#elif NET472 || NET48
        public HateoasShould()
        {
            _data = new TestData();
            _mockHateoasContext = new Mock<IHateoasContext>().SetupAllProperties();
        }

        [Fact]
        public void Generate_HttpActionDescriptorNull_ThrowsArgumentNullException()
        {
            // arrange
            TestData.Faker.Populate(_data, TestData.RuleSets.PrefixOnly);
            MockHateoasContext();
            // not mocking ActionDescriptor resulting on it being null
            _sut = new Hateoas(_mockHateoasContext.Object);

            // act & assert
            _ = Assert.Throws<NotSupportedException>(() => _sut.Generate(_data.RouteValues));
        }

        [Fact]
        public void Generate_InvalidParameterNotInRouteValues_ThrowsException()
        {
            // arrange
            TestData.Faker.Populate(_data, TestData.RuleSets.ParameterNotInRouteValues);
            MockHateoasContext();
            MockHttpContextAndActionDescriptors();
            _sut = new Hateoas(_mockHateoasContext.Object);

            // act & assert
            _ = Assert.Throws<InvalidOperationException>(() => _sut.Generate(_data.RouteValues));
        }

        private void MockHttpContextAndActionDescriptors()
        {
            var config = new Mock<HttpConfiguration>().Object;
            var prefix = new RoutePrefixAttribute(_data.Prefix);
            var controllerDescriptor = new TestControllerDescriptor(config, _data.ControllerName, typeof(ApiController), prefix);
            var actionParameters =
                _data.QueryStrings.Aggregate(new Collection<HttpParameterDescriptor>(),
                (collection, parameterName) =>
                {
                    var mockParameterDescriptor = new Mock<HttpParameterDescriptor>();
                    _ = mockParameterDescriptor.Setup(x => x.ParameterName).Returns(parameterName);
                    collection.Add(mockParameterDescriptor.Object);
                    return collection;
                });
            var methodInfo = new TestMethodInfo(new RouteAttribute(_data.Template) { Name = _data.RouteName });
            var actionDescriptor = new TestActionDescriptor(controllerDescriptor, methodInfo, actionParameters);
            actionDescriptor.SupportedHttpMethods.Add(_data.Method);
            var dataTokens = new RouteValueDictionary
            {
                { "descriptors", new[] { actionDescriptor } }
            };

            RouteTable.Routes.Add(new Route(_data.RoutePath, null) { DataTokens = dataTokens });
            var request = new HttpRequest(string.Empty, _data.BaseUrl, string.Empty);
            var response = new HttpResponse(new StringWriter());
            HttpContext.Current = new HttpContext(request, response);
        }
#endif
    }
}
