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
    }
}