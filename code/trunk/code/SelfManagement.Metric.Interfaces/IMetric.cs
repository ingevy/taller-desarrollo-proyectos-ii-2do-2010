namespace CallCenter.SelfManagement.Metric.Interfaces
{
    using System.Collections.Generic;
    using System;

    public interface IMetric
    {
        IDictionary<int, double> CalculatedValues { get; }

        DateTime MetricDate { get; }

        void ProcessFiles(IList<IDataFile> dataFiles);
    }
}
