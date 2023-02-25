// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Bogus;

namespace HateoasNet.Tests.TestHelpers
{
    public class HateoasTestData
    {
        private static readonly HttpMethod[] s_methods = new[] { HttpMethod.Get, HttpMethod.Post, HttpMethod.Put, HttpMethod.Delete, };
        public static Faker<HateoasTestData> Fake(Dictionary<string, object> routeValues = null) => new Faker<HateoasTestData>()
            .Rules((f, h) =>
            {
                h.RouteValues = routeValues ?? new Dictionary<string, object>();
                h.Method = f.Random.CollectionItem(s_methods);
                h.BaseUrl = f.Internet.Url();
                h.ControllerName = f.Lorem.Word();
                h.Prefix = $"{f.Internet.DomainWord()}/{h.ControllerName}";
                h.RoutePath = h.RouteValues.Any()
                    ? $"{h.Prefix}/{string.Join("/", h.RouteValues.Keys.Select(key => $"{key}/{h.RouteValues[key]}"))}"
                    : $"{h.Prefix}";
                h.ExpectedUrl = $"{h.BaseUrl}/{h.RoutePath}";
                h.Template = string.Join("/", h.RouteValues.Keys.Select(key => $"{key}/{{{key}}}"));
                h.RouteName = $"{h.Method.Method.ToLower()}-{h.ControllerName.ToLower()}";
            });

        public IDictionary<string, object> RouteValues { get; internal set; }
        public HttpMethod Method { get; internal set; }
        public string BaseUrl { get; internal set; }
        public string ControllerName { get; internal set; }
        public string Prefix { get; internal set; }
        public string RoutePath { get; internal set; }
        public string ExpectedUrl { get; internal set; }
        public string Template { get; internal set; }
        public string RouteName { get; internal set; }
    }
}
