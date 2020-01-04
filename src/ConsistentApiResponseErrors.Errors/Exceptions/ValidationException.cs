using ConsistentApiResponseErrors.ConsistentErrors;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsistentApiResponseErrors.Exceptions
{
    /// <summary>
    /// ValidationException is used to throw custom validation errors. 
    /// </summary>
    [Serializable]
    public class ValidationException : Exception
    {
        public ValidationError ValidationResult { get; private set; }
        public List<ValidationFailure> ValidationFailures { get; private set; } = new List<ValidationFailure>();

        /// <summary>
        /// This <see cref="ValidationException"></see> can be thrown based on the <see cref="FluentValidation.Results.ValidationResult"></see> Errors.
        /// </summary>
        /// <param name="validationFailures"></param>
        /// <param name="traceId"></param>
        public ValidationException(IList<ValidationFailure> validationFailures, string traceId)
                : base()
        {
            ValidationResult = new ValidationError(validationFailures, traceId);
        }

        /// <summary>
        /// This <see cref="ValidationException"></see> can be thrown based on the <see cref="FluentValidation.Results.ValidationResult"></see> Errors.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="validationFailures"></param>
        /// <param name="traceId"></param>
        public ValidationException(string errorMessage, IList<ValidationFailure> validationFailures, string traceId)
            : base(errorMessage)
        {
            ValidationResult = new ValidationError(validationFailures, traceId);
        }

        /// <summary>
        /// This <see cref="ValidationException"></see> can be thrown based on the <see cref="FluentValidation.Results.ValidationResult"></see> Errors with Exception wrapping.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="validationFailures"></param>
        /// <param name="traceId"></param>
        /// <param name="innerException"></param>
        public ValidationException(string errorMessage, IList<ValidationFailure> validationFailures, string traceId, Exception innerException)
            : base(errorMessage, innerException)
        {
            ValidationResult = new ValidationError(validationFailures, traceId);
        }


        /// <summary>
        /// This <see cref="ValidationException"></see> can be thrown based on the ModelState.
        /// The <c>ModelState.IsValid</c> should be <c>False</c> to use <see cref="ValidationException"></see>.
        /// </summary>
        /// <param name="errorMessage">The Exception Error</param>
        /// <param name="errorCode">e.g. "bad_request"</param>
        /// <param name="modelStateDictionary"></param>
        /// <param name="traceId"></param>
        public ValidationException(string errorMessage, string errorCode, ModelStateDictionary modelStateDictionary, string traceId)
        : base(errorMessage)
        {
            foreach (string modelStateKey in modelStateDictionary.Keys)
            {
                var modelState = modelStateDictionary[modelStateKey];
                if (modelState.Errors.Count <= 0)
                {
                    continue;
                }

                foreach (ModelError modelError in modelState.Errors)
                {
                    ValidationFailure validationFailure = new ValidationFailure(modelStateKey, modelError.ErrorMessage, modelState.AttemptedValue);
                    validationFailure.ErrorCode = errorCode;

                    ValidationFailures.Add(validationFailure);
                }
            }
            ValidationResult = new ValidationError(ValidationFailures, traceId);

        }

        /// <summary>
        /// This <see cref="ValidationException"></see> can be thrown based on the ModelState.
        /// The <c>ModelState.IsValid</c> should be <c>False</c> to use <see cref="ValidationException"></see>.
        /// </summary>
        /// <param name="errorCode">e.g. "bad_request"</param>
        /// <param name="modelStateDictionary"></param>
        /// <param name="traceId"></param>
        public ValidationException(string errorCode, ModelStateDictionary modelStateDictionary, string traceId) 
            : this(string.Empty, errorCode, modelStateDictionary, traceId)
        {
        }
    }
}
