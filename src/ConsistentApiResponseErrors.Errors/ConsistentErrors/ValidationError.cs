﻿using ConsistentApiResponseErrors.Helpers.Enums;
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
        public ValidationError()
        {
            this.StatusCode = 0;
            this.StatusMessage = String.Empty;
            this.TraceId = string.Empty;
            Errors = new List<Error>();
        }

        public ValidationError(IList<ValidationFailure> ValidationFailures, string TraceId)
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
        public IList<Error> Errors { get; set; }
    }

}
