using ConsistentApiResponseErrors.Exceptions;
using ConsistentApiResponseErrors.Helpers.Enums;
using System;
using System.Net;

namespace ConsistentApiResponseErrors.ConsistentErrors
{
    /// <summary>
    /// The selected consistent response for Business Login and Unhandled Exceptions (similar to the ValidationError)
    /// </summary>
    [Serializable]
    public class ExceptionError
    {
        public ExceptionError(Exception exception, string TraceId)
        {
            // Exception type To HTTP Status configuration 
            switch (exception)
            {
                case ApiBaseException apiException when exception is ApiBaseException:
                    this.StatusCode = apiException.StatusCode;
                    this.StatusMessage = apiException.StatusMessage;
                    this.StackTrace = apiException.StackTrace;
                    break;
                default:
                    this.StatusCode = (int)HttpStatusCode.InternalServerError;
                    this.StatusMessage = HttpStatusCode.InternalServerError.ToStringSpaceCamelCase();
                    this.StackTrace = exception.StackTrace;
                    break;
            }

            this.ErrorMessage = exception.GetBaseException().Message;
            this.TraceId = TraceId;
        }

        public ExceptionError(int StatusCode, string StatusMessage, Exception exception, string TraceId)
        {
            this.StatusCode = StatusCode;
            this.StatusMessage = StatusMessage;
            this.ErrorMessage = exception.GetBaseException().Message;
            this.StackTrace = exception.StackTrace;
            this.TraceId = TraceId;
        }

        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string ErrorMessage { get; set; }
        public string TraceId { get; set; }
        public string StackTrace { internal get; set; }
    }

}
