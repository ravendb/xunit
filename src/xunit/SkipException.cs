using System;
using System.Runtime.Serialization;

namespace Xunit
{
    [Serializable]
    public class SkipException : Exception
    {
        public SkipException()
        {
        }

        public SkipException(string message) : base(message)
        {
        }

        public SkipException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SkipException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}