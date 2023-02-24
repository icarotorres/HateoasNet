// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET472 || NET48
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace HateoasNet.Tests.TestHelpers
{
    /// <summary>
    ///   Custom dummy of HttpActionDescriptor abstract class to get RouteAttribute
    ///   and HttpMethod from a Controller Action MethodInfo for testing purposes
    /// </summary>
    internal class TestActionDescriptor : HttpActionDescriptor
    {
        private readonly Collection<HttpParameterDescriptor> _parameterDescriptors;

        public TestActionDescriptor(HttpControllerDescriptor controllerDescriptor,
                                    MethodInfo methodInfo,
                                    Collection<HttpParameterDescriptor> parameterDescriptors) : base(controllerDescriptor)
        {
            _parameterDescriptors = parameterDescriptors;
            MethodInfo = methodInfo;
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public MethodInfo MethodInfo { get; }

        /// <inheritdoc />
        public override string ActionName => "TestAction";

        /// <inheritdoc />
        public override Type ReturnType => typeof(object);

        /// <inheritdoc />
        public override Collection<HttpParameterDescriptor> GetParameters()
        {
            return _parameterDescriptors;
        }

        /// <inheritdoc />
        public override Collection<T> GetCustomAttributes<T>()
        {
            return new Collection<T>();
        }

        /// <inheritdoc />
        public override Task<object> ExecuteAsync(HttpControllerContext controllerContext, IDictionary<string, object> arguments, CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
#endif
