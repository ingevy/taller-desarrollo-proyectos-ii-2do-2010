namespace CallCenter.SelfManagement.Metric.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IDataFile
    {
        ExternalSystemFiles ExternalSystemFile { get; }

        DateTime FileDate { get; }

        string FilePath { get; }

        IList<Dictionary<string,string>> DataLines { get; }
    }
}
