using System;
using System.Runtime.Serialization;

namespace BloodyBoss.Exceptions
{
    internal class NPCDontExistException : Exception
    {
        public NPCDontExistException()
        {
        }

        public NPCDontExistException(string message)
            : base(message)
        {
        }

        public NPCDontExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NPCDontExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
