using System;

namespace PM.MVC.Models.Exceptions
{
    //Catch the exception in service layer and rewrap it into a custom exception
    [Serializable]
    public class RequestedResourceHasConflictException : Exception
    {
        public RequestedResourceHasConflictException()
        {
        }

        public RequestedResourceHasConflictException(string message)
            : base(message)
        {
        }

        public RequestedResourceHasConflictException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}