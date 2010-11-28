namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using CallCenter.SelfManagement.Web.Helpers;

    [PropertiesMustMatch("NewPassword", "ConfirmPassword", ErrorMessage = "La nueva contraseña y la confirmación de la misma no coinciden.")]
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "La contraseña actual es requerida.")]
        [DataType(DataType.Password)]
        [DisplayName("Contraseña actual")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida.")]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("Nueva contraseña")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "La confirmación de la contraseña es requerida.")]
        [DataType(DataType.Password)]
        [DisplayName("Confirmar nueva contraseña")]
        public string ConfirmPassword { get; set; }
    }
}