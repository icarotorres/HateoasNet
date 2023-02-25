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
using System.Text;
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

#if NET7_0 || NETCOREAPP3_1
        private readonly Mock<IUrlHelper> _mockUrlHelper;
        private readonly Mock<IActionDescriptorCollectionProvider> _mockActionDescriptorCollectionProvider;

        public HateoasShould()
        {
            _mockHateoasContext = new Mock<IHateoasContext>().SetupAllProperties();
            _mockActionDescriptorCollectionProvider = new Mock<IActionDescriptorCollectionProvider>().SetupAllProperties();
            _mockUrlHelper = new Mock<IUrlHelper>().SetupAllProperties();
        }

        private void BuildSutDependencies<T>(T data, string routeName, string url, string method) where T : class
        {
            _ = _mockHateoasContext
                .SetupAllProperties()
                .Setup(x => x.GetApplicableLinkBuilders(data))
                .Returns(new IHateoasLinkBuilder[] { new HateoasLinkBuilder<T>(routeName) });

            _ = _mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(url);

            var actionDescriptors = new List<ActionDescriptor>
            {
                new ActionDescriptor
                {
                    AttributeRouteInfo = new AttributeRouteInfo {Name = routeName},
                    ActionConstraints = new List<IActionConstraintMetadata>
                        {new HttpMethodActionConstraint(new List<string> {method})}
                }
            };
            _ = _mockActionDescriptorCollectionProvider
                .Setup(x => x.ActionDescriptors)
                .Returns(new ActionDescriptorCollection(actionDescriptors, 1));

            _sut = new Hateoas(_mockHateoasContext.Object, _mockUrlHelper.Object, _mockActionDescriptorCollectionProvider.Object);
        }

        [Theory]
        [HateoasCoreData]
        public void Generate_ValidParameters_ReturnsHateoasLinks<T>(T data, string routeName, string url, string method) where T : class
        {
            // arrange
            var expected = new HateoasLink(routeName, url, method);
            BuildSutDependencies(data, routeName, url, method);

            // act
            var links = _sut.Generate(data).ToArray();

            // assert
            _ = links.Should()
                .NotBeEmpty().And
                .BeAssignableTo<IEnumerable<HateoasLink>>().And
                .BeEquivalentTo(new HateoasLink[] { expected });
        }

#elif NET472 || NET48
        public HateoasShould()
        {
            _mockHateoasContext = new Mock<IHateoasContext>().SetupAllProperties();
        }

        [Theory]
        [HateoasFrameworkData]
        public void Generate_ValidParameters_ReturnsHateoasLinks(HateoasTestData mockData)
        {
            // arrange
            var expected = new HateoasLink(mockData.RouteName, mockData.ExpectedUrl, mockData.Method.Method);
            MockHateoasContext(mockData);
            MockHttpContextAndActionDescriptors(mockData);
            _sut = new Hateoas(_mockHateoasContext.Object);

            // act
            var links = _sut.Generate(mockData.RouteValues).ToArray();

            // assert
            _ = links.Should()
                .NotBeEmpty().And
                .BeAssignableTo<IEnumerable<HateoasLink>>().And
                .BeEquivalentTo(new HateoasLink[] { expected });
        }

        [Theory]
        [InlineData(
            "id,cpf,admin",
            "10/12345678900/false",
            "{id:int}/cpf/{cpf:length(11)}/admin/{admin:bool}",
            "10/cpf/12345678900/admin/false")]
        [InlineData(
            "id,name,active",
            "D3CF664E-5267-4D17-9DCA-3374307C122A/sample_user/true",
            "{id:guid}/name/{name:maxlength(15)}/active/{active:bool}",
            "D3CF664E-5267-4D17-9DCA-3374307C122A/name/sample_user/active/true")]
        public void BuildRouteParameters_RouteValuesWithRouteParameter_BuildExpectedString(string keys, string values, string routeTemplate, string expected)
        {
            // arrange
            var randomPrefixUrl = $"https://Random/{Guid.NewGuid()}/prefix/";
            var routeUrlBuilder = new StringBuilder(randomPrefixUrl);
            var keyArray = keys.Split(',');
            var valuesArray = values.Split('/');
            var routeValues = new Dictionary<string, object>(keyArray.Length);
            foreach (var (key, value) in keyArray.Zip(valuesArray, (k, v) => (k, v)))
            {
                routeValues[key] = value;
            }
            _sut = new Hateoas(_mockHateoasContext.Object);

            // act
            _sut.BuildRouteParameters(routeUrlBuilder, routeTemplate, routeValues);
            var actual = routeUrlBuilder.ToString();

            // assert
            actual.Should().Contain(expected);
        }

        [Theory]
        [InlineData("{keyNotInRouteValues}", "keyInRouteValues")]
        [InlineData("{keyInRouteValues}/{keyNotInRouteValues}", "keyInRouteValues")]
        public void BuildRouteParameters_RouteValuesMissingRouteParameter_ThrowsException(string routeTemplate, string key)
        {
            // arrange
            var routeValues = new Dictionary<string, object> { [key] = null };
            _sut = new Hateoas(_mockHateoasContext.Object);

            // assert
            _ = Assert.Throws<InvalidOperationException>(() => _sut.BuildRouteParameters(new StringBuilder(), routeTemplate, routeValues));
        }

        private void MockHateoasContext(HateoasTestData mockData)
        {
            var mockLinkBuilder = new Mock<IHateoasLinkBuilder<Dictionary<string, object>>>();
            _ = mockLinkBuilder.Setup(x => x.RouteName).Returns(mockData.RouteName);
            _ = mockLinkBuilder.Setup(x => x.PresentedName).Returns(mockData.RouteName);
            _ = mockLinkBuilder.Setup(x => x.GetRouteDictionary(It.IsAny<object>())).Returns(mockData.RouteValues);

            _ = _mockHateoasContext
                .SetupAllProperties()
                .Setup(x => x.GetApplicableLinkBuilders(mockData.RouteValues))
                .Returns(new IHateoasLinkBuilder[] { mockLinkBuilder.Object });
        }

        private void MockHttpContextAndActionDescriptors(HateoasTestData mockData)
        {
            var config = new Mock<HttpConfiguration>().Object;
            var prefix = new RoutePrefixAttribute(mockData.Prefix);
            var controllerDescriptor = new TestControllerDescriptor(config, mockData.ControllerName, typeof(ApiController), prefix);
            var actionParameters =
                mockData.RouteValues.Keys.Aggregate(new Collection<HttpParameterDescriptor>(),
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
