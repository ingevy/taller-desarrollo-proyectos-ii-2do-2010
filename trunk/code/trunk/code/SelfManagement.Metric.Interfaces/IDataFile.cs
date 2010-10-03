namespace CallCenter.SelfManagement.Metric.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IDataFile
    {
        string ExternalSystemName { get; }

        DateTime FileDate { get; }

        string FilePath { get; }

        List<string[]> DataLines { get; }
    }
}
