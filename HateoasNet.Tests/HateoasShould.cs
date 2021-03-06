using HateoasNet.Abstractions;
using HateoasNet.Tests.TestHelpers;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using FluentAssertions;
#if NETCOREAPP3_1
using HateoasNet.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
#elif NET472
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

#if NETCOREAPP3_1
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
            _mockHateoasContext
                .SetupAllProperties()
                .Setup(x => x.GetApplicableLinkBuilders(data))
                .Returns(new IHateoasLinkBuilder[] { new HateoasLinkBuilder<T>(routeName) });

            _mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(url);

            var actionDescriptors = new List<ActionDescriptor>
            {
                new ActionDescriptor
                {
                    AttributeRouteInfo = new AttributeRouteInfo {Name = routeName},
                    ActionConstraints = new List<IActionConstraintMetadata>
                        {new HttpMethodActionConstraint(new List<string> {method})}
                }
            };
            _mockActionDescriptorCollectionProvider
                .Setup(x => x.ActionDescriptors)
                .Returns(new ActionDescriptorCollection(actionDescriptors, 1));

            _sut = new Hateoas(_mockHateoasContext.Object, _mockUrlHelper.Object, _mockActionDescriptorCollectionProvider.Object);
        }

        [Theory]
        [HateoasCoreData]
        [Trait(nameof(IHateoas), nameof(IHateoas.Generate))]
        public void Generate_ValidParameters_ReturnsHateoasLinks<T>(T data, string routeName, string url, string method) where T : class
        {
            // arrange
            var expected = new HateoasLink(routeName, url, method);
            BuildSutDependencies(data, routeName, url, method);

            // act
            var links = _sut.Generate(data).ToArray();

            // assert
            links.Should()
                .NotBeEmpty().And
                .BeAssignableTo<IEnumerable<HateoasLink>>().And
                .BeEquivalentTo(new HateoasLink[] { expected });
        }

#elif NET472
        public HateoasShould()
        {
            _mockHateoasContext = new Mock<IHateoasContext>().SetupAllProperties();
        }

        [Theory]
        [HateoasFrameworkData]
        [Trait(nameof(IHateoas), nameof(IHateoas.Generate))]
        public void Generate_ValidParameters_ReturnsHateoasLinks(HateoasTestData mockData)
        {
            // arrange
            var expected = new HateoasLink(mockData.RouteName, mockData.ExpectedUrl, mockData.Method);
            MockSutDependencies(mockData);

            // act
            var links = _sut.Generate(mockData.RouteValues).ToArray();

            // assert
            links.Should()
                .NotBeEmpty().And
                .BeAssignableTo<IEnumerable<HateoasLink>>().And
                .BeEquivalentTo(new HateoasLink[] { expected });
        }

        [Fact]
        [Trait(nameof(IHateoas), nameof(Hateoas.HandleRouteTemplate))]
        public void Generate_InvalidActionDescriptor_ThrowsEception()
        {
            // arrange
            var mockData = new HateoasTestData(
                "sample-exception",
                "exceptions",
                "GET",
                new Dictionary<string, object> { { "diferentKey", "diferentValue" } },
                "{otherValue}");
            MockHateoasContext(mockData);
            _sut = new Hateoas(_mockHateoasContext.Object);

            // act
            Action actual = () => _sut.HandleRouteTemplate(mockData.ExpectedUrl, mockData.Template, mockData.RouteValues);

            // assert
            Assert.Throws<InvalidOperationException>(actual);
        }

        private void MockSutDependencies(HateoasTestData mockData)
        {
            MockHateoasContext(mockData);
            MockHttpContextAndActionDescriptors(mockData);
            _sut = new Hateoas(_mockHateoasContext.Object);
        }

        private void MockHateoasContext(HateoasTestData mockData)
        {
            var mockLinkBuilder = new Mock<IHateoasLinkBuilder<Dictionary<string, object>>>();
            mockLinkBuilder.Setup(x => x.RouteName).Returns(mockData.RouteName);
            mockLinkBuilder.Setup(x => x.PresentedName).Returns(mockData.RouteName);
            mockLinkBuilder.Setup(x => x.GetRouteDictionary(It.IsAny<object>())).Returns(mockData.RouteValues);

            _mockHateoasContext
                .SetupAllProperties()
                .Setup(x => x.GetApplicableLinkBuilders(mockData.RouteValues))
                .Returns(new IHateoasLinkBuilder[] { mockLinkBuilder.Object });
        }

        private void MockHttpContextAndActionDescriptors(HateoasTestData mockData)
        {
            var config = new Mock<HttpConfiguration>().Object;
            var prefix = new RoutePrefixAttribute(mockData.Prefix);
            var controllerDescriptor =
                new TestControllerDescriptor(config, mockData.ControllerName, typeof(ApiController), prefix);
            var actionParameters =
                mockData.RouteValues.Keys.Aggregate(new Collection<HttpParameterDescriptor>(),
                (collection, parameterName) =>
                {
                    var mockParameterDescriptor = new Mock<HttpParameterDescriptor>();
                    mockParameterDescriptor.Setup(x => x.ParameterName).Returns(parameterName);
                    collection.Add(mockParameterDescriptor.Object);
                    return collection;
                });
            var methodInfo = new TestMethodInfo(new RouteAttribute(mockData.Template) { Name = mockData.RouteName });
            var actionDescriptor = new TestActionDescriptor(controllerDescriptor, methodInfo, actionParameters);
            actionDescriptor.SupportedHttpMethods.Add(new HttpMethod(mockData.Method));
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
