using System;

namespace ConsistentApiResponseErrors.Exceptions
{
    /// <summary>
    /// Business Login Exceptions, mapped with http error codes to be used at thr API layer
    /// </summary>
    [Serializable]
    public class ApiBaseException : Exception
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public ApiBaseException(int StatusCode, string StatusMessage, string ErrorMessage) : base(ErrorMessage)
        {
            this.StatusCode = StatusCode;
            this.StatusMessage = StatusMessage;
        }
    }
}
