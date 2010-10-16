namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using CallCenter.SelfManagement.Web.Helpers;

    [PropertiesMustMatch("Password", "ConfirmPassword", ErrorMessage = "La contraseña y la confirmación de la misma no coinciden.")]
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        [DisplayName("Nombre de usuario")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "El e-mail es requerido.")]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "La confirmación de la contraseña es requerida.")]
        [DataType(DataType.Password)]
        [DisplayName("Confirmar contraseña")]
        public string ConfirmPassword { get; set; }
    }
}