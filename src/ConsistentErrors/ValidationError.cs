using ConsistentApiResponseErrors.Helpers.Enums;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Net;

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
            this.StatusCode = (int)HttpStatusCode.BadRequest;
            this.StatusMessage = HttpStatusCode.BadRequest.ToStringSpaceCamelCase();
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
    }

}
