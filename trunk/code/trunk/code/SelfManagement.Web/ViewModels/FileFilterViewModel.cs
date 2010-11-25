namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System.ComponentModel;

    public class FileFilterViewModel
    {
        [DisplayName("Fecha Datos:")]
        public string DateData { get; set; }

        [DisplayName("Fecha Procesado:")]
        public string DateProcessed { get; set; }

        [DisplayName("Fecha Modificado:")]
        public string DateModified { get; set; }

        [DisplayName("Tipo:")]
        public int FileType { get; set; }

        [DisplayName("Estado:")]
        public int State { get; set; }
    }
}