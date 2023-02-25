// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using HateoasNet.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
#if NET7_0 || NETCOREAPP3_1
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;
#elif NET472 || NET48
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Routing;
using System.Web.UI;
#endif

namespace HateoasNet
{
    /// <inheritdoc cref="IHateoas" />
    public sealed class Hateoas : IHateoas
    {
        private readonly IHateoasContext _context;

        public IEnumerable<HateoasLink> Generate(object source)
        {
            foreach (var linkBuilder in _context.GetApplicableLinkBuilders(source))
            {
                yield return CreateHateoasLink(linkBuilder.RouteName, linkBuilder.GetRouteDictionary(source), linkBuilder.PresentedName);
            }
        }
#if NET7_0 || NETCOREAPP3_1
        private readonly IReadOnlyList<ActionDescriptor> _actionDescriptors;
        private readonly IUrlHelper _urlHelper;

        public Hateoas(IHateoasContext context, IUrlHelper urlHelper, IActionDescriptorCollectionProvider provider)
        {
            _context = context;
            _urlHelper = urlHelper;
            _actionDescriptors = provider.ActionDescriptors.Items;
        }

        private HateoasLink CreateHateoasLink(string routeName, IDictionary<string, object> routeValues, string presentedName)
        {
            if (string.IsNullOrWhiteSpace(routeName))
            {
                throw new ArgumentNullException(nameof(routeName));
            }

            if (!TryGetActionDescriptorByRouteName(routeName, out var actionDescriptor))
            {
                throw new NotSupportedException($"Unable to find route '{routeName}' and respective {nameof(ActionDescriptor)}");
            }

            var href = _urlHelper.Link(routeName, routeValues);
            var method = actionDescriptor.ActionConstraints.OfType<HttpMethodActionConstraint>().First().HttpMethods.First();
            return new HateoasLink(presentedName, href, method);
        }

        private bool TryGetActionDescriptorByRouteName(string routeName, out ActionDescriptor descriptor)
        {
            descriptor = _actionDescriptors.SingleOrDefault(x => x.AttributeRouteInfo.Name == routeName);

            return descriptor != null;
        }
#elif NET472 || NET48
        private static readonly Regex s_keyFromRouteRegex = new(@"\{(.*?)\}", RegexOptions.Compiled, TimeSpan.FromSeconds(10));
        private static readonly Regex s_keyFromParameterConstraintsRegex = new(@"\w(\w|\d|_)*", RegexOptions.Compiled, TimeSpan.FromSeconds(10));
        private static readonly RouteAttribute s_dummyRouteAttributeInCaseNotFound = new("unable-to-find-route");

        public Hateoas(IHateoasContext context)
        {
            _context = context;
        }

        internal HateoasLink CreateHateoasLink(string routeName, IDictionary<string, object> routeValues, string presentedName)
        {
            if (string.IsNullOrWhiteSpace(routeName))
            {
                throw new ArgumentNullException(nameof(routeName));
            }

            var routeActionDescriptors = GetRouteActionDescriptors(RouteTable.Routes);
            var href = GetRouteUrl(routeName, routeValues, routeActionDescriptors);
            var method = GetRouteMethod(routeName, routeActionDescriptors);
            return new HateoasLink(presentedName, href, method);
        }

        internal Dictionary<RouteAttribute, HttpActionDescriptor> GetRouteActionDescriptors(RouteCollection routes)
        {
            var actionDescriptors = routes
                .OfType<Route>()
                .Select(route => route.DataTokens.Values.OfType<HttpActionDescriptor[]>()
                .FirstOrDefault()?.First()).Where(x => x != null);

            return actionDescriptors.ToDictionary(GetRouteAttributeOrDefault);
        }

        internal RouteAttribute GetRouteAttributeOrDefault(HttpActionDescriptor descriptor)
        {
            var methodInfo = descriptor.GetType().GetProperty(nameof(MethodInfo))?.GetValue(descriptor) as MethodInfo;

            return methodInfo?.GetCustomAttributes(true).OfType<RouteAttribute>().FirstOrDefault()
                   ?? s_dummyRouteAttributeInCaseNotFound;
        }

        /// <summary>
        ///   Builds an url <see langword="string" /> with the <paramref name="routeName" /> and <paramref name="routeValues" />.
        /// </summary>
        /// <param name="routeName">Name of desired route to discover the url.</param>
        /// <param name="routeValues">Route dictionary to look for parameters and query strings.</param>
        /// <returns>Generated Url <see langword="string" /> value.</returns>
        private string GetRouteUrl(string routeName, IDictionary<string, object> routeValues, Dictionary<RouteAttribute, HttpActionDescriptor> routeActionDescriptors)
        {
            if (HttpContext.Current.Request == null)
            {
                throw new NotSupportedException($"Not supported execution without a current {nameof(HttpContext.Current.Request)}.");
            }

            var (routeAttribute, descriptor) = routeActionDescriptors
                .Where(pair => pair.Key.Name == routeName)
                .Select(x => (x.Key, x.Value)).First();

            if (descriptor == null)
            {
                throw new NotSupportedException($"Not found the '{nameof(descriptor)}' for route with name '{routeName}'.");
            }

            var resourceUrlBuilder = new StringBuilder();

            BuildUrlSegments(resourceUrlBuilder, descriptor, HttpContext.Current.Request);
            BuildUrlScheme(resourceUrlBuilder, HttpContext.Current.Request);

            if (!string.IsNullOrWhiteSpace(routeAttribute.Template) && routeValues != null)
            {                
                BuildRouteParameters(resourceUrlBuilder, routeAttribute.Template, routeValues);
                BuildQueryStrings(resourceUrlBuilder, routeAttribute.Template, routeValues, descriptor);
            }

            return resourceUrlBuilder.ToString();
        }

        private void BuildUrlSegments(StringBuilder resourceUrlBuilder, HttpActionDescriptor descriptor, HttpRequest request)
        {
            resourceUrlBuilder
                .Append(request.Url.Authority)
                .Append("/")
                .Append(request.ApplicationPath)
                .Append("/")
                .Append(GetResourceName(descriptor))
                .Replace("//", "/");
        }

        private string GetResourceName(HttpActionDescriptor descriptor)
        {
            var controllerDescriptor = descriptor.ControllerDescriptor;
            var routePrefixAttribute = controllerDescriptor.GetCustomAttributes<RoutePrefixAttribute>().FirstOrDefault();
            return routePrefixAttribute != null ? routePrefixAttribute.Prefix : controllerDescriptor.ControllerName;
        }

        private void BuildUrlScheme(StringBuilder resourceUrlBuilder, HttpRequest request)
        {
            resourceUrlBuilder.Insert(0, "://").Insert(0, request.Url.Scheme);
        }

        /// <summary>
        ///   Find the <see langword="string" /> value of HTTP method of a route with given <paramref name="routeName" />.
        /// </summary>
        /// <param name="routeName">The wanted endpoint route name to find.</param>
        /// <returns><see langword="string" />value representing HTTP method.</returns>
        private string GetRouteMethod(string routeName, Dictionary<RouteAttribute, HttpActionDescriptor> routeActionDescriptors)
        {
            var descriptor = routeActionDescriptors.Single(pair => pair.Key.Name == routeName).Value;
            return descriptor.SupportedHttpMethods.FirstOrDefault().Method ??
                throw new InvalidOperationException($"Unable to get '{nameof(HttpMethod)}' needed to create the link.");
        }

        internal void BuildRouteParameters(StringBuilder resourceUrlBuilder, string template, IDictionary<string, object> routeValues)
        {
            resourceUrlBuilder.Append("/").Append(template);
            foreach (Match match in s_keyFromRouteRegex.Matches(template))
            {
                var keyFromRouteParameter = match.Value.Replace("{", "").Replace("}", "");
                var parameterNotFoundOrHasConstraints = !routeValues.TryGetValue(keyFromRouteParameter, out var replacement);

                if (parameterNotFoundOrHasConstraints)
                {
                    var keyFromParameterWithConstraints = s_keyFromParameterConstraintsRegex.Matches(match.Value)[0].Value;
                    if (!routeValues.TryGetValue(keyFromParameterWithConstraints, out replacement))
                    {
                        throw new InvalidOperationException($"Unable to find key '{keyFromParameterWithConstraints}' from dictionary of route values.");
                    }
                }
                resourceUrlBuilder.Replace(match.Value, replacement?.ToString());
            }
        }

        private void BuildQueryStrings(StringBuilder resourceUrlBuilder, string template, IDictionary<string, object> routeValues, HttpActionDescriptor descriptor)
        {
            var parameterCounter = 0;
            foreach (var parameter in descriptor.GetParameters())
            {
                if (routeValues.ContainsKey(parameter.ParameterName) && !template.Contains($"{{{parameter.ParameterName}"))
                {
                    var symbol = parameterCounter == 0 ? "?" : "&";
                    resourceUrlBuilder.Append(symbol).Append(routeValues[parameter.ParameterName]);
                    parameterCounter++;
                }
            }
        }
#endif
    }
}
