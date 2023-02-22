// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Web;
using System.Web.Http;

namespace HateoasNet.Framework.Sample
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(config =>
            {
                RouteConfig.RegisterRoutes(config);
            });
        }
    }
}
