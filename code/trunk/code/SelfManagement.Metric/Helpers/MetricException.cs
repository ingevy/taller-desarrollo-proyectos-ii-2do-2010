namespace CallCenter.SelfManagement.Metric.Helpers
{
    public class MetricException : System.ApplicationException
    {
        public MetricException() : base() { }
        public MetricException(string message) : base(message) { }
        public MetricException(string message, System.Exception inner) : base(message, inner) { }

        // Constructor needed for serialization 
        // when exception propagates from a remoting server to the client.
        protected MetricException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}