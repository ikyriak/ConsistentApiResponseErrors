using System;
using ConsistentApiResponseErrors.Exceptions;

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
            // Exception type To Http Status configuration 
            switch (exception)
            {
                case ApiBaseException apiException when exception is ApiBaseException:
                    this.StatusCode = apiException.StatusCode;
                    this.StatusMessage = apiException.StatusMessage;
                    this.StackTrace = string.Empty;
                    break;
                default:
                    this.StatusCode = 500;
                    this.StatusMessage = "Internal Server Error";
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
        public string StackTrace { get; set; }
    }

}
