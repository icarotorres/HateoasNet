// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET472
using System;
using System.Collections.ObjectModel;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace HateoasNet.Tests.TestHelpers
{
    public class TestControllerDescriptor : HttpControllerDescriptor
    {
        public TestControllerDescriptor(HttpConfiguration configuration, string controllerName, Type controllerType, RoutePrefixAttribute prefix)
            : base(configuration, controllerName, controllerType)
        {
            RoutePrefix = prefix;
        }

        public RoutePrefixAttribute RoutePrefix { get; }

        public override Collection<T> GetCustomAttributes<T>() where T : class
        {
            return typeof(T) == RoutePrefix.GetType()
                ? new Collection<T> { RoutePrefix as T }
                : base.GetCustomAttributes<T>();
        }
    }
}
#endif