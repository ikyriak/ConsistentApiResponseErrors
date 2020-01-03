using ConsistentApiResponseErrors.Helpers.Enums;
using System;
using System.Net;

namespace ConsistentApiResponseErrors.Exceptions
{
    /// <summary>
    /// Business Login Exceptions, mapped with HTTP Status Codes to be used at the API layer
    /// </summary>
    [Serializable]
    public class ApiBaseException : Exception
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        /// <summary>
        /// An ApiBaseException mapping manually the HTTP Status Codes
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="statusMessage"></param>
        /// <param name="errorMessage"></param>
        public ApiBaseException(int statusCode, string statusMessage, string errorMessage)
            : base(errorMessage)
        {
            this.StatusCode = statusCode;
            this.StatusMessage = statusMessage;
        }

        /// <summary>
        /// An ApiBaseException mapping the HTTP Status Codes using the System.Net.HttpStatusCode Enum
        /// </summary>
        /// <param name="httpStatusCode"></param>
        /// <param name="errorMessage"></param>
        public ApiBaseException(HttpStatusCode httpStatusCode, string errorMessage)
            : base(errorMessage)
        {
            this.StatusCode = (int)httpStatusCode;
            this.StatusMessage = httpStatusCode.ToStringSpaceCamelCase();
        }

        /// <summary>
        /// An ApiBaseException with Exception wrapping
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="statusMessage"></param>
        /// <param name="errorMessage"></param>
        /// <param name="innerException"></param>
        public ApiBaseException(int statusCode, string statusMessage, string errorMessage, Exception innerException)
            : base(errorMessage, innerException)
        {
            this.StatusCode = statusCode;
            this.StatusMessage = statusMessage;
        }

        /// <summary>
        /// An ApiBaseException with Exception wrapping and using the HttpStatusCode Enum.
        /// </summary>
        /// <param name="httpStatusCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="innerException"></param>
        public ApiBaseException(HttpStatusCode httpStatusCode, string errorMessage, Exception innerException)
            : base(errorMessage, innerException)
        {
            this.StatusCode = (int)httpStatusCode;
            this.StatusMessage = httpStatusCode.ToStringSpaceCamelCase();
        }

        /// <summary>
        /// An ApiBaseException Exception wrapping with a 500 HTTP Status Code (Internal Server Error)
        /// </summary>
        /// <param name="ErrorMessage"></param>
        /// <param name="ex"></param>
        public ApiBaseException(string errorMessage, Exception innerException)
            : this (HttpStatusCode.InternalServerError, errorMessage, innerException)
        {
        }        
    }
}
