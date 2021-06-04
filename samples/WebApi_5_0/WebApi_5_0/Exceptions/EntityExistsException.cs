using System;
using ConsistentApiResponseErrors.Exceptions;

namespace WebApi_5_0.Exceptions
{
    [Serializable]
    public class EntityExistsException : ApiBaseException
    {
        private const int _httpStatusCode = 409;
        private const string _httpStatusMessage = "Conflict";
        private const string _DefaultErrorMessage = "Entity exists";

        public EntityExistsException() :
            base(_httpStatusCode, _httpStatusMessage, _DefaultErrorMessage)
        {
        }

        public EntityExistsException(string message)
           : base(_httpStatusCode, _httpStatusMessage, message)
        {
        }

        public EntityExistsException(string message, Exception innerException)
            : base(_httpStatusCode, _httpStatusMessage, message, innerException)
        {
        }
    }
}
