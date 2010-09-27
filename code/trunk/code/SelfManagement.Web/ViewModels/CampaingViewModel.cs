namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CampaingViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CustomerName { get; set; }

        public int CampaingType { get; set; }

        public int SupervisorId { get; set; }

        public string BeginDate { get; set; }

        public string EndDate { get; set; }

        public string Description { get; set; }

        public IList<SupervisorViewModel> Supervisors { get; set; }

        public IList<MetricViewModel> Metrics { get; set; }
    }
}