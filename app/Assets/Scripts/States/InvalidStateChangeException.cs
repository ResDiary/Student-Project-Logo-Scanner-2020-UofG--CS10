using System;

namespace States
{
    public class InvalidStateChangeException : Exception
    {
        public InvalidStateChangeException()
        {
        }

        public InvalidStateChangeException(string message)
            : base(message)
        {
        }

        public InvalidStateChangeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}