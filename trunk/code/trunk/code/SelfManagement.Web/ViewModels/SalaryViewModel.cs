namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System.ComponentModel;

    public class SalaryViewModel
    {
        [DisplayName("Sueldo Bruto")]
        public string GrossSalary { get; set; }

        [DisplayName("Parte Variable Proyectada")]
        public string VariableSalary { get; set; }

        [DisplayName("Sueldo Total Proyectado")]
        public string TotalSalary { get; set; }

        [DisplayName("Sueldo Horas Extra 50% Proyectado")]
        public string Extra50Salary { get; set; }

        [DisplayName("Sueldo Horas Extra 100% Proyectado")]
        public string Extra100Salary { get; set; }

        [DisplayName("Total Horas Proyectadas")]
        public int TotalHoursWorked { get; set; }

        [DisplayName("Horas Extra 50% Proyectadas")]
        public int ExtraHours50Worked { get; set; }

        [DisplayName("Horas Extra 100% Proyectadas")]
        public int ExtraHours100Worked { get; set; }

        [DisplayName("Horas Extra 50% Trabajadas")]
        public int CurrentExtraHours50Worked { get; set; }

        [DisplayName("Horas Extra 100% Trabajadas")]
        public int CurrentExtraHours100Worked { get; set; }
    }
}