namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class LogOnViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        [DisplayName("Nombre de usuario")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [DataType(DataType.Password)]
        [DisplayName("Contraseña")]
        public string Password { get; set; }

        [DisplayName("Recordar mi contraseña")]
        public bool RememberMe { get; set; }
    }

}