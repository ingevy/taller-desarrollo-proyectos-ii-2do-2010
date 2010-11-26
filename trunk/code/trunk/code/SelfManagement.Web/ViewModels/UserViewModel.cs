namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using CallCenter.SelfManagement.Web.Helpers;
    using CallCenter.SelfManagement.Data;

    [PropertiesMustMatch("Password", "ConfirmPassword", ErrorMessage = "La contraseña y la confirmación de la misma no coinciden.")]
    public class UserViewModel
    {
        private const NumberStyles NumberStyle = NumberStyles.Float | NumberStyles.Integer | NumberStyles.Number | NumberStyles.Currency;

        [CustomValidation(typeof(UserViewModel), "ValidateNumber")]
        [DisplayName("Identificador:")]
        public string Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        [StringLength(256, ErrorMessage="El nombre de usurio debe tener menos de 256 caracteres.")]
        [DisplayName("Nombre de Usuario:")]
        public string Username { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida.")]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "La confirmación de la contraseña es requerida.")]
        [DataType(DataType.Password)]
        [DisplayName("Confirmar contraseña")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Rol de Usuario:")]
        [Range(0, 3, ErrorMessage = "El rol seleccionado es inválido.")]
        public int Role { get; set; }

        [Required(ErrorMessage = "El nombre del usuario es requerido.")]
        [DisplayName("Nombres:")]
        public string Names { get; set; }

        [Required(ErrorMessage = "El apellido del usuario es requerido.")]
        [DisplayName("Apellido:")]
        public string LastName { get; set; }

        [DisplayName("Estado:")]
        [Range(0, 1, ErrorMessage = "El estado seleccionado es inválido.")]
        public int Status { get; set; }

        [Required(ErrorMessage = "El nombre del usuario es requerido.")]
        [DisplayName("DNI:")]
        public string Dni { get; set; }

        [Required(ErrorMessage = "El e-mail es requerido.")]
        [DisplayName("E-mail:")]
        public string Email { get; set; }

        [DisplayName("Salario Bruto:")]
        [CustomValidation(typeof(UserViewModel), "ValidateNumber")]
        public string GrossSalary { get; set; }

        [Range(1, 1, ErrorMessage = "El salario bruto del usuario es requerido.")]
        public int GrossSalaryRequired
        {
            get
            {
                if (this.Role == (int)SelfManagementRoles.Agent)
                {
                    return string.IsNullOrWhiteSpace(this.GrossSalary) ? 0 : 1;
                }

                return 1;
            }
        }

        public string GlobalError { get; set; }

        public static ValidationResult ValidateNumber(string number, ValidationContext validationContext)
        {
            double result = 0;
            var format = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            format.CurrencySymbol = "$";

            if (!double.TryParse(number, NumberStyle, format, out result))
            {
                return new ValidationResult("Formato inválido.");
            }

            if (result <= 0)
            {
                return new ValidationResult("El valor debe ser mayor a cero.");
            }

            return ValidationResult.Success;
        }
    }
}