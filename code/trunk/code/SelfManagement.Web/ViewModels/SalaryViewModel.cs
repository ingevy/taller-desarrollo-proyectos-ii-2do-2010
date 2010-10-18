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
        public double GrossSalary { get; set; }

        [DisplayName("Parte Variable Proyectada")]
        public double VariableSalary { get; set; }

        [DisplayName("Sueldo Total Proyectado")]
        public double TotalSalary { get; set; }
    }
}