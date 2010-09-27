using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CallCenter.SelfManagement.Web.ViewModels
{
    public class MetricViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int FormatType { get; set; }
    }
}