namespace CallCenter.SelfManagement.Web.Helpers
{
    using System.Linq;
    using System.Security.Principal;
    using System.Web.Security;

    public static class ExtensionMethodHelper
    {
        public static string GetRole(this IPrincipal principal)
        {
            var role = Roles.GetRolesForUser().FirstOrDefault();

            return role ?? "Sin rol asignado";
        }
    }
}