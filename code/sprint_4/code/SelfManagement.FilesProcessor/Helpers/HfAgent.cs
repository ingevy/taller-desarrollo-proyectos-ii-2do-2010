namespace CallCenter.SelfManagement.FilesProcessor.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    class HfAgent
    {
        public int Legajo { get; set; }
        public string DNI { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public decimal Salary { get; set; }
        public string Workday { get; set; }
        public string Status { get; set; }
        public int SupervisorId { get; set; }
        public DateTime IncorporationDate { get; set; }
        public int CampaingId { get; set; }

        public HfAgent(IDictionary<string, string> properties)
        {
            var strFecha = properties[" fecha ingreso"].Split('/');

            Legajo = Convert.ToInt32(properties["legajo"]);
            DNI = properties["dni"];
            Name = properties["nombre"];
            LastName = properties["apellido"];
            Salary = Convert.ToDecimal(properties["sueldo bruto"]);
            Workday = properties["Tipo Jornada"];
            Status = properties[" Status"];
            SupervisorId = Convert.ToInt32(properties["idSupervisor"]);
            CampaingId = Convert.ToInt32(properties[" idCampania"]);
            IncorporationDate = new DateTime(Convert.ToInt32(strFecha[2]), //Año
                                            Convert.ToInt32(strFecha[1]), //Mes
                                            Convert.ToInt32(strFecha[0])); //Dia
        }

    }
}
