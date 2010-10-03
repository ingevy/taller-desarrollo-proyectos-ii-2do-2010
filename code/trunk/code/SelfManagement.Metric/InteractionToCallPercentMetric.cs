namespace CallCenter.SelfManagement.Metric
{
    using System.Collections.Generic;
    using CallCenter.SelfManagement.Metric.Interfaces;

    public class InteractionToCallPercentMetric : IMetric
    {
        private IDictionary<int, double> calculatedValues = new Dictionary<int, double>();
        private string valueType = "Percent";

        public IDictionary<int, double> CalculatedValues
        {
            get { return this.calculatedValues; }
        }

        public string ValueType
        {
            get { return this.valueType; }
        }

        public void ProcessFiles(IList<IDataFile> dataFiles)
        {
            throw new System.NotImplementedException();
        }
    }
}
