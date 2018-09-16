using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AndroidEmulatorController
{
    class TargetNotFoundException : Exception, ISerializable
    {
        public TargetNotFoundException()
        : base("Target Not Found!!") { }
        public TargetNotFoundException(string message) 
        : base(message) { }
        public TargetNotFoundException(string message, Exception inner) 
        : base(message, inner) { }
        protected TargetNotFoundException(SerializationInfo info, StreamingContext context) 
        : base(info, context) { }
    }
}
