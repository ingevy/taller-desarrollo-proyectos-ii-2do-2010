using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CallCenter.SelfManagement.FilesProcessor.Helpers
{
    class STSException : System.ApplicationException
    {
        public STSException() : base() {}
        public STSException(string message) : base(message) {}
        public STSException(string message, System.Exception inner) : base(message, inner) {}
 
        // Constructor needed for serialization 
        // when exception propagates from a remoting server to the client.
        protected STSException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) {}
    }
}
