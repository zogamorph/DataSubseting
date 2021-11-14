using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core
{
    [Serializable()]
    public class InvalidParameterException : Exception
    {
        public string InvalidParameterMessage { get; set; }

        public InvalidParameterException(string invalidParameterMessages) : base()
        {
            InvalidParameterMessage = invalidParameterMessages;
        }

        public InvalidParameterException(string message, string invalidParameterMessages) : base(message)
        {
            InvalidParameterMessage = invalidParameterMessages;
        }

        public InvalidParameterException(string message, string invalidParameterMessages, System.Exception inner)
            : base(message, inner)
        {
            InvalidParameterMessage = invalidParameterMessages;
        }
    
        public InvalidParameterException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
