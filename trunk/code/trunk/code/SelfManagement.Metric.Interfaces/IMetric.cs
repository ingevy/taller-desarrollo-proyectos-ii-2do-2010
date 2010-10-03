namespace CallCenter.SelfManagement.Metric.Interfaces
{
    using System.Collections.Generic;
    using Callcenter.SelfManagement.Metric.Interfaces;

    public interface IMetric
    {
        void ProcessFiles(IList<IDataFile> dataFiles);

        IDictionary<int, double> GetCalculatedValues();

        string GetValueType();
    }
}
