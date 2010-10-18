using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace CallCenter.SelfManagement.Web.ViewModels
{
    public class SalaryViewModel
    {
        [DisplayName("Sueldo bruto")]
        public double GrossSalary { get; set; }

        [DisplayName("Parte variable proyectada")]
        public double VariableSalary { get; set; }

        [DisplayName("Sueldo total proyectado")]
        public double TotalSalary { get; set; }
    }
}