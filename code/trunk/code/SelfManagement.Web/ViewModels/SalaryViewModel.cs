using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace CallCenter.SelfManagement.Web.ViewModels
{
    public class SalaryViewModel
    {
        [DisplayName("Sueldo Bruto")]
        public string GrossSalary { get; set; }

        [DisplayName("Parte Variable Proyectada")]
        public string VariableSalary { get; set; }

        [DisplayName("Sueldo Total Proyectado")]
        public string TotalSalary { get; set; }

        [DisplayName("Horas Extra 50% Proyectadas")]
        public string Extra50Salary { get; set; }

        [DisplayName("Horas Extra 100% Proyectadas")]
        public string Extra100Salary { get; set; }

        [DisplayName("Total Horas")]
        public int TotalHoursWorked { get; set; }

        [DisplayName("Horas Extra 50%")]
        public int ExtraHours50Worked { get; set; }

        [DisplayName("Horas Extra 100%")]
        public int ExtraHours100Worked { get; set; }
    }
}