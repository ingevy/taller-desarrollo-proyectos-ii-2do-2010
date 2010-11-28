namespace CallCenter.SelfManagement.Web
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

            // Supervisors
            routes.MapRoute(
                "SearchSupervisors",
                "Supervisor/Search/{searchCriteria}",
                new { controller = "Supervisor", action = "Search" });

            // Campaings
            routes.MapRoute(
                "SearchCampaings",
                "Campaing/Search/{searchCriteria}",
                new { controller = "Campaing", action = "Search" });

            routes.MapRoute(
                "EditCampaing",
                "Campaing/Edit/{campaingId}",
                new { controller = "Campaing", action = "Edit" });

            routes.MapRoute(
               "EndCampaing",
               "Campaing/End/{campaingId}",
               new { controller = "Campaing", action = "End" });

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