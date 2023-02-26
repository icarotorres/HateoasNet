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

        public static Faker<HateoasTestData> Valid(int parametersCount, int queryStringsCount) => new Faker<HateoasTestData>().Rules((f, h) =>
        {
            PopulateRouteValuesAndQueryStrings(f, h, parametersCount, queryStringsCount);
            var routeParameters = h.RouteValues.Keys.Except(h.QueryStrings);
            h.Method = f.Random.CollectionItem(s_methods);
            h.BaseUrl = f.Internet.Url();
            h.ControllerName = f.Internet.DomainWord();
            h.Prefix = $"{f.Internet.DomainWord()}/{h.ControllerName}";
            h.RoutePath = $"{h.Prefix}{GenerateUrlPartFromParameters(routeParameters, h.RouteValues)}";
            h.QueryStringPath = GenerateUrlPartFromQueryStrings(h.QueryStrings, h.RouteValues);
            h.Template = GenerateRouteTemplateFromParameters(routeParameters);
            h.ExpectedUrl = $"{h.BaseUrl}/{h.RoutePath}{h.QueryStringPath}";
            h.RouteName = $"{f.Hacker.Verb().ToLower()}-{h.ControllerName.ToLower()}";
        });

        public static Faker<HateoasTestData> InvalidParameterNotInRouteValues(string parameter) => Valid(0, 0).Rules((f, h) =>
        {
            h.Template = GenerateRouteTemplateFromParameters(new[] { parameter });
        });


        private static void PopulateRouteValuesAndQueryStrings(Faker f, HateoasTestData h, int parameterCount, int queryStringCount)
        {
            h.RouteValues = new Dictionary<string, object>(parameterCount + queryStringCount);
            for (var i = 0; i < parameterCount; i++)
            {
                h.RouteValues.Add(GenerateKey(f), f.Random.UShort());
            }

            h.QueryStrings = new HashSet<string>(queryStringCount);
            for (var i = 0; i < queryStringCount; i++)
            {
                var query = GenerateKey(f);
                h.QueryStrings.Add(query);
                h.RouteValues.Add(query, f.Hacker.Adjective());
            }
        }

        private static string GenerateKey(Faker f)
        {
            return $"{f.Random.AlphaNumeric(2)}-{f.Hacker.Adjective()}";
        }

        private static string GenerateRouteTemplateFromParameters(IEnumerable<string> parameters)
        {
            return string.Join("/", parameters.Select(key => $"{key}/{{{key}}}"));
        }

        private static string GenerateUrlPartFromParameters(IEnumerable<string> parameters, IDictionary<string, object> values)
        {
            return parameters.Any()
                ? $"/{string.Join("/", parameters.Select(key => $"{key}/{values[key]}"))}"
                : string.Empty;
        }

        private static string GenerateUrlPartFromQueryStrings(IEnumerable<string> queryStrings, IDictionary<string, object> values)
        {
            return queryStrings.Any()
                ? $"?{string.Join("&", queryStrings.Select(key => $"{key}={values[key]}"))}"
                : string.Empty;
        }

        public IDictionary<string, object> RouteValues { get; internal set; }
        public HashSet<string> QueryStrings { get; internal set; }
        public HttpMethod Method { get; internal set; }
        public string BaseUrl { get; internal set; }
        public string ControllerName { get; internal set; }
        public string Prefix { get; internal set; }
        public string Template { get; internal set; }
        public string RoutePath { get; internal set; }
        public string QueryStringPath { get; internal set; }
        public string ExpectedUrl { get; internal set; }
        public string RouteName { get; internal set; }
    }
}
