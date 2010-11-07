namespace CallCenter.SelfManagement.Metric.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IMetric
    {
        IDictionary<int, double> CalculatedValues { get; }

        DateTime MetricDate { get; }

        void ProcessFiles(IList<IDataFile> dataFiles);
    }
}
