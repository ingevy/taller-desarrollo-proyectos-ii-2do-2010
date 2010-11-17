using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CallCenter.SelfManagement.Web.ViewModels
{
    public class SupervisorViewModel
    {
        public int Id { get; set; }

        public string DisplayName { get; set; }

        public bool Selected { get; set; }
    }
}