using System;
using System.Runtime.Serialization;

namespace BloodyBoss.Exceptions
{
    internal class BossExistException : Exception
    {
        public BossExistException()
        {
        }

        public BossExistException(string message)
            : base(message)
        {
        }

        public BossExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected BossExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
