namespace CallCenter.SelfManagement.Web.Helpers
{
    using System.Web.Security;

    public static class AccountValidation
    {
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "El nombre de usuario ya existe. Por favor, ingrese uno diferente.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Un nombre de usuario ya existe para el e-mail seleccionado. Por favor, ingrese una dirección de email diferente.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "La dirección de e-mail es inválido. Por favor, revise el valor y pruebe nuevamente.";

                case MembershipCreateStatus.InvalidUserName:
                    return "El nombre de usuario es inválido. Por favor, revise el valor y pruebe nuevamente.";

                case MembershipCreateStatus.ProviderError:
                    return "El proveedor de autenticación retornó un error. Por favor, revise el valor y pruebe nuevamente.";

                case MembershipCreateStatus.UserRejected:
                    return "La creación del usuario ha sido cancelada. Por favor, revise el valor y pruebe nuevamente.";

                default:
                    return "Ocurrió un error desconocido. Por favor, verifique sus datos e intente nuevamente.";
            }
        }
    }
}