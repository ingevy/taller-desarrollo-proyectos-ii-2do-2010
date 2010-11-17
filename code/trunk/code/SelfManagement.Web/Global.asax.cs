﻿namespace CallCenter.SelfManagement.Web
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("Content/{*pathInfo}");

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Agents
            routes.MapRoute(
                "SearchAgents",
                "Agent/Search/{searchCriteria}",
                new { controller = "Agent", action = "Search" });

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }
    }
}