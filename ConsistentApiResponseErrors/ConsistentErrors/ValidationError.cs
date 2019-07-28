using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace ConsistentApiResponseErrors.ConsistentErrors
{
    /// <summary>
    /// The selected consistent response for the validation of the request models (similar to the ExceptionError)
    /// </summary>
    [Serializable]
    public class ValidationError
    {
        public ValidationError(List<ValidationFailure> ValidationFailures, string TraceId)
        {
            this.StatusCode = 400;
            this.StatusMessage = "Bad Request";
            this.TraceId = TraceId;

            Errors = new List<Error>();
            foreach (ValidationFailure validationFailure in ValidationFailures)
            {
                Errors.Add(new Error(validationFailure));
            }
        }

        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string TraceId { get; set; }
        public List<Error> Errors { get; set; }


        [Serializable]
        public class Error
        {
            public Error()
            {
                Code = string.Empty;
                Field = string.Empty;
                AttemptedValue = string.Empty;
                Message = string.Empty;
                HelpURL = string.Empty;
            }

            public Error(ValidationFailure validationFailure)
            {
                Code = validationFailure.ErrorCode;
                Field = validationFailure.PropertyName;
                AttemptedValue = validationFailure.AttemptedValue;
                Message = validationFailure.ErrorMessage;

                // TODO: To be implemented!
                HelpURL = string.Empty;
            }

            public string Code { get; set; }
            public string Field { get; set; }
            public object AttemptedValue { get; set; }
            public string Message { get; set; }
            public string HelpURL { get; set; }
        }
    }

}
