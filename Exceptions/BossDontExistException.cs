using System;
using System.Runtime.Serialization;

namespace BloodyBoss.Exceptions
{
    internal class BossDontExistException : Exception
    {
        public BossDontExistException()
        {
        }

        public BossDontExistException(string message)
            : base(message)
        {
        }

        public BossDontExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected BossDontExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
