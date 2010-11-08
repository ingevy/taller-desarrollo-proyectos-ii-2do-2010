namespace CallCenter.SelfManagement.Web.Helpers
{
    using System.Linq;
    using System.Security.Principal;
    using System.Web.Security;
    using CallCenter.SelfManagement.Web.ViewModels;

    public static class ExtensionMethodHelper
    {
        public static string GetRole(this IPrincipal principal)
        {
            var role = Roles.GetRolesForUser().FirstOrDefault();

            return role ?? "Sin rol asignado";
        }

        public static string GetCssClass(this MetricLevel metriclevel)
        {
            return metriclevel.ToString().ToLowerInvariant();
        }

        public static string GetDescription(this MetricLevel metriclevel)
        {
            switch (metriclevel)
            {
                case MetricLevel.Optimal: return "Optimo";
                case MetricLevel.Objective: return "Objetivo";
                case MetricLevel.Minimum: return "Minimo";
                default:
                    return "No Satisfactorio";
            }
        }
    }
}