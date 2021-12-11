using System;
using System.Runtime.Serialization;

namespace NorthwindDAL
{
    [Serializable]
    public class InvalidOrderStateException : Exception
    {
        public InvalidOrderStateException()
        {
        }

        public InvalidOrderStateException(string message) : base(message)
        {
        }

        public InvalidOrderStateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidOrderStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}