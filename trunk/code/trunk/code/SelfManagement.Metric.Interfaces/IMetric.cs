namespace CallCenter.SelfManagement.Metric.Interfaces
{
    using System.Collections.Generic;

    public interface IMetric
    {
        IDictionary<int, double> CalculatedValues { get; }

        string ValueType { get; }

        void ProcessFiles(IList<IDataFile> dataFiles);
    }
}
