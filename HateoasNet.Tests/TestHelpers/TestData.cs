// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Bogus;

namespace HateoasNet.Tests.TestHelpers
{
    public class TestData
    {
        public static class RuleSets
        {
            public const string Base = "Base";
            public const string PrefixOnly = "PrefixOnly,Base";
            public const string RouteTemplate = "RouteTemplate,Base";
            public const string QueryStrings = "QueryStrings,Base";
            public const string Complete = "RouteTemplate,QueryStrings,Base";
            public const string RouteNameNull = "RouteNameNull";
            public const string ParameterNotInRouteValues = "PrefixOnly,Base,ParameterNotInRouteValues";
        }

        private static readonly HttpMethod[] s_methods = new[] { HttpMethod.Get, HttpMethod.Post, HttpMethod.Put, HttpMethod.Delete, };

        public static Faker<TestData> Faker => new Faker<TestData>()
            .RuleSet(nameof(RuleSets.PrefixOnly), x => GenerateQueryStringAndRouteValuesRule(x, queryStringCount: 0, parameterCount: 0))
            .RuleSet(nameof(RuleSets.RouteTemplate), x => GenerateQueryStringAndRouteValuesRule(x, queryStringCount: 0, parameterCount: 2))
            .RuleSet(nameof(RuleSets.QueryStrings), x => GenerateQueryStringAndRouteValuesRule(x, queryStringCount: 2, parameterCount: 0))
            .RuleSet(nameof(RuleSets.Complete), x => GenerateQueryStringAndRouteValuesRule(x, queryStringCount: 2, parameterCount: 2))
            .RuleSet(nameof(RuleSets.Base), x => x.Rules((f, d) =>
            {
                var routeParameters = d.RouteValues.Keys.Except(d.QueryStrings);
                d.Method = f.Random.CollectionItem(s_methods);
                d.BaseUrl = f.Internet.Url();
                d.ControllerName = f.Internet.DomainWord();
                d.Prefix = $"{f.Internet.DomainWord()}/{d.ControllerName}";
                d.RoutePath = $"{d.Prefix}{GenerateUrlPartFromParameters(routeParameters, d.RouteValues)}";
                d.QueryStringPath = GenerateUrlPartFromQueryStrings(d.QueryStrings, d.RouteValues);
                d.Template = GenerateRouteTemplateFromParameters(routeParameters);
                d.ExpectedUrl = $"{d.BaseUrl}/{d.RoutePath}{d.QueryStringPath}";
                d.RouteName = $"{f.Hacker.Verb().ToLower()}-{d.ControllerName.ToLower()}";
            }))
            .RuleSet(nameof(RuleSets.RouteNameNull), x => x.RuleFor(d => d.RouteName, f => null))
            .RuleSet(nameof(RuleSets.ParameterNotInRouteValues), x => x.RuleFor(d => d.Template,
                f => GenerateRouteTemplateFromParameters(new[] { GenerateKey(f) })));

        private static void GenerateQueryStringAndRouteValuesRule(IRuleSet<TestData> x, int queryStringCount, int parameterCount)
        {
            x.Rules((f, d) =>
            {
                d.QueryStrings = GenerateQueryStrings(f, queryStringCount);
                d.RouteValues = GenerateRouteValues(f, parameterCount, d.QueryStrings);
            });
        }

        private static HashSet<string> GenerateQueryStrings(Faker f, int queryStringCount)
        {
            return Enumerable.Range(0, queryStringCount).Select(x => GenerateKey(f)).ToHashSet();
        }

        private static Dictionary<string, object> GenerateRouteValues(Faker f, int parameterCount, HashSet<string> queryStrings)
        {
            var routeParameters = Enumerable.Range(0, parameterCount).Select(x => GenerateKey(f)).ToHashSet();
            return routeParameters.Union(queryStrings).ToDictionary(x => x, x => (object)f.Random.UShort());
        }

        private static string GenerateKey(Faker f)
        {
            return $"{f.Random.AlphaNumeric(1)}-{f.Hacker.Adjective()}";
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
