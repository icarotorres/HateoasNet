// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Net.Http;

namespace HateoasNet.Tests.TestHelpers
{
    public class Testdata
    {
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

