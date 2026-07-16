using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MvcExample.Infrastructure;
using NLog;

namespace MvcExample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            try
            {
                SecretsManagerAccessor.Initialize();
            }
            catch (Exception ex)
            {
                // Don't let composition-root wiring failures take down the whole app domain —
                // requests that actually need the secrets service will fail individually
                // (SecretsManagerAccessor throws a clear "not initialized" error) instead of
                // every request 500ing until this is fixed and the app pool recycled.
                Log.Error(ex, "Failed to initialize SecretsManagerAccessor");
            }

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}