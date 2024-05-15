using System;
using System.Runtime.Serialization;

namespace BloodyBoss.Exceptions
{
    internal class ProductDontExistException : Exception
    {
        public ProductDontExistException()
        {
        }

        public ProductDontExistException(string message)
            : base(message)
        {
        }

        public ProductDontExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ProductDontExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
