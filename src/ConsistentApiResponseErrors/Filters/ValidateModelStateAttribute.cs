﻿using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using FluentValidation;
using FluentValidation.Results;
using ConsistentApiResponseErrors.ConsistentErrors;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace ConsistentApiResponseErrors.Filters
{
    // <summary>
    /// Introduces Model state auto validation to reduce code duplication
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute" />
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        private readonly IValidatorFactory _validatorFactory;
        private readonly ILogger _logger;

        public ValidateModelStateAttribute(IValidatorFactory validatorFactory, ILogger<ValidateModelStateAttribute> logger)
        {
            _validatorFactory = validatorFactory;

            if (logger != null)
            {
                _logger = logger;
            }
            else
            {
                _logger = NullLogger.Instance;
            }
        }

        /// <summary>
        /// Validates Model automatically 
        /// </summary>
        /// <param name="context"></param>
        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string traceId = Guid.NewGuid().ToString();
            _logger.LogInformation("({traceId}): Start validation OnActionExecuting", traceId);

            if (context == null)
            {
                Exception exception = new Exception("ConsistentApiResponseErrors ValidateModelStateAttribute: The context is null");
                _logger.LogError(exception, exception.Message + " ({traceId})", traceId);
                
                throw exception;
            }


            List<ValidationFailure> allErrors = new List<ValidationFailure>();

            // Validate the basic model state
            if (!context.ModelState.IsValid)
            {
                foreach(string modelStateKey in context.ModelState.Keys)
                {
                    var modelState = context.ModelState[modelStateKey];
                    if (modelState.Errors.Count <= 0)
                    {
                        continue;
                    }

                    foreach (ModelError modelError in modelState.Errors)
                    {
                        ValidationFailure validationFailure = new ValidationFailure(modelStateKey, modelError.ErrorMessage, modelState.AttemptedValue);
                        validationFailure.ErrorCode = "bad_request";
                        
                        allErrors.Add(validationFailure);
                    }
                }
            }
            else if(context.ActionArguments != null)
            {
                // Get the fluent validator for each argument and perform validation:
                foreach (KeyValuePair<string, object> ActionArgument in context.ActionArguments)
                {
                    if (ActionArgument.Value == null)
                    {
                        Exception exception = new Exception("ConsistentApiResponseErrors ValidateModelStateAttribute: The ActionArgument.Value is null");
                        _logger.LogError(exception, exception.Message + " ({traceId})", traceId);
                        
                        throw exception;
                    }

                    IValidator validator = _validatorFactory.GetValidator(ActionArgument.Value.GetType());
                    if (validator == null)
                    {
                        // Add log if no validator is found:
                        _logger.LogWarning("{traceId}: The validator is null for type {ActionArgumentType}", traceId, ActionArgument.Value.GetType().ToString());
                        continue;
                    }

                    ValidationResult result = validator.Validate(ActionArgument.Value);
                    if (!result.IsValid)
                    {
                        allErrors.AddRange(result.Errors);
                    }

                    // Add log for the validation result:
                    _logger.LogInformation("({traceId}): The validation result: {IsResultValid} for value \"{ActionArgumentValue}\"", traceId, result.IsValid.ToString(), ActionArgument.Value.ToString());
                }
            }

            // When errors exist return a BadRequestObjectResult
            if (allErrors.Count > 0)
            {
                // Log validation errors with this specific traceID:
                _logger.LogWarning("({traceId}): Validation Errors: {ValidationErrors}", traceId, allErrors);

                context.Result = new BadRequestObjectResult(new ValidationError(allErrors, traceId));
            }

            // Add log for the validation result:
            _logger.LogInformation("({traceId}): OnActionExecuting: End validation", traceId);
        }
    }

}
