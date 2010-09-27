namespace CallCenter.SelfManagement.Web.Helpers
{
    using System.Security.Principal;

    public static class ExtensionMethodHelper
    {
        public static string GetRole(this IPrincipal principal)
        {
            // TODO: Refactor this code getting the current user role from a repository
            if (principal.IsInRole("AccountManager")) return "Jefe de Cuentas";
            if (principal.IsInRole("Agent")) return "Agente";
            if (principal.IsInRole("ITManager")) return "Responsable de IT";
            if (principal.IsInRole("Supervisor")) return "Supervisor";

            return "Desconocido";
        }
    }
}