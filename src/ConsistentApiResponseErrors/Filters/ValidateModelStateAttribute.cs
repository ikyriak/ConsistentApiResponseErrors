using System;
using System.Collections.Generic;
using ConsistentApiResponseErrors.ConsistentErrors;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
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
                // 2020-01-04: Use the "Exceptions.ValidationException" to convert ModelState to Fluent-ValidationFailures
                Exceptions.ValidationException validationException = new Exceptions.ValidationException("bad_request", context.ModelState, traceId);
                allErrors.AddRange(validationException.ValidationFailures);
            }
            else if (context.ActionArguments != null)
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
