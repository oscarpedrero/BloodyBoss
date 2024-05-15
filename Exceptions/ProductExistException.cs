using System;
using System.Runtime.Serialization;

namespace BloodyBoss.Exceptions
{
    internal class ProductExistException : Exception
    {
        public ProductExistException()
        {
        }

        public ProductExistException(string message)
            : base(message)
        {
        }

        public ProductExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ProductExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
