namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System.ComponentModel;

    public class FileFilterViewModel
    {
        [DisplayName("Fecha Datos:")]
        public string DataDate { get; set; }

        [DisplayName("Fecha Procesado:")]
        public string ProcessingDate { get; set; }

        [DisplayName("Fecha Modificado:")]
        public string ModifiedDate { get; set; }

        [DisplayName("Tipo:")]
        public int Type { get; set; }

        [DisplayName("Estado:")]
        public int State { get; set; }
    }
}