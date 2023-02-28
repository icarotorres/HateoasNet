// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET472 || NET48
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Routing;
using HateoasNet.Tests.TestHelpers;
using Moq;

namespace HateoasNet.Tests.TestHelpers
{
    public static class NetFrameworkStaticMocks
    {
        public static void MockHttpContextAndActionDescriptors(Testdata data)
        {
            var config = new Mock<HttpConfiguration>().Object;
            var prefix = new RoutePrefixAttribute(data.Prefix);
            var controllerDescriptor = new TestControllerDescriptor(config, data.ControllerName, typeof(ApiController), prefix);
            var actionParameters =
                data.QueryStrings.Aggregate(new Collection<HttpParameterDescriptor>(),
                (collection, parameterName) =>
                {
                    var mockParameterDescriptor = new Mock<HttpParameterDescriptor>();
                    _ = mockParameterDescriptor.Setup(x => x.ParameterName).Returns(parameterName);
                    collection.Add(mockParameterDescriptor.Object);
                    return collection;
                });
            var methodInfo = new TestMethodInfo(new RouteAttribute(data.Template) { Name = data.RouteName });
            var actionDescriptor = new TestActionDescriptor(controllerDescriptor, methodInfo, actionParameters);
            actionDescriptor.SupportedHttpMethods.Add(data.Method);
            var dataTokens = new RouteValueDictionary
            {
                { "descriptors", new[] { actionDescriptor } }
            };

            RouteTable.Routes.Add(new Route(data.RoutePath, null) { DataTokens = dataTokens });
            var request = new HttpRequest(string.Empty, data.BaseUrl, string.Empty);
            var response = new HttpResponse(new StringWriter());
            HttpContext.Current = new HttpContext(request, response);
        }
    }
}
#endif
