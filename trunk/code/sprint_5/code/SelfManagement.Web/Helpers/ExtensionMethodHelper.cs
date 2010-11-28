namespace CallCenter.SelfManagement.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Web.Security;
    using CallCenter.SelfManagement.Web.ViewModels;

    public static class ExtensionMethodHelper
    {
        public static string GetRole(this IPrincipal principal)
        {
            var role = Roles.GetRolesForUser().FirstOrDefault();

            return GetRoleDisplayName(role);
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

        public static IEnumerable<string> GetRoles()
        {
            return Roles.GetAllRoles().Select(r => GetRoleDisplayName(r));
        }

        public static string GetCssClass(this MetricLevel metriclevel)
        {
            return metriclevel.ToString().ToLowerInvariant();
        }

        private static string GetRoleDisplayName(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return "Sin rol asignado";
            }

            if (role.Equals("AccountManager", StringComparison.OrdinalIgnoreCase))
            {
                return "Jefe de Cuentas";
            }

            if (role.Equals("Supervisor", StringComparison.OrdinalIgnoreCase))
            {
                return "Supervisor";
            }

            if (role.Equals("Agent", StringComparison.OrdinalIgnoreCase))
            {
                return "Agente";
            }

            if (role.Equals("ITManager", StringComparison.OrdinalIgnoreCase))
            {
                return "Responsable de IT";
            }

            return "Desconocido";
        }
    }
}