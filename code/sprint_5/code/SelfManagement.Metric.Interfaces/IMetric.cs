namespace CallCenter.SelfManagement.Metric.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IMetric
    {
        IDictionary<int, double> CalculatedValues { get; }

        DateTime MetricDate { get; }

        IList<ExternalSystemFiles> ExternalFilesNeeded { get; }

        void ProcessFiles(IList<IDataFile> dataFiles);
    }
}
