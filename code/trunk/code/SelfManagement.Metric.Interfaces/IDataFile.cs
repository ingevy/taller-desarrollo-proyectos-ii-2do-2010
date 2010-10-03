namespace CallCenter.SelfManagement.Metric.Interfaces
{
    using System;

    public interface IDataFile
    {
        string ExternalSystemName { get; set; }

        DateTime FileDate { get; set; }

        string FilePath { get; set; }
    }
}
