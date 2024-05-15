using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BloodyBoos.Exceptions
{
    internal class NPCExistException : Exception
    {
        public NPCExistException()
        {
        }

        public NPCExistException(string message)
            : base(message)
        {
        }

        public NPCExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NPCExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
