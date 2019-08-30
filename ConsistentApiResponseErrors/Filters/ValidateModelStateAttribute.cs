using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using FluentValidation;
using FluentValidation.Results;
using ConsistentApiResponseErrors.ConsistentErrors;

namespace ConsistentApiResponseErrors.Filters
{
    // <summary>
    /// Introduces Model state auto validation to reduce code duplication
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute" />
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        private readonly IValidatorFactory _validatorFactory;

        public ValidateModelStateAttribute(IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory;
        }

        /// <summary>
        /// Validates Model automaticaly 
        /// </summary>
        /// <param name="context"></param>
        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context == null || context.ActionArguments == null)
            {
                // Console Log (+++)
                return;
            }

            // Get the validator for each argument and perform validation:
            List<ValidationFailure> allErrors = new List<ValidationFailure>();
            foreach (KeyValuePair<string, object> ActionArgument in context.ActionArguments)
            {
                if (ActionArgument.Value == null)
                {
                    // Console Log (+++)
                    continue;
                }

                IValidator validator = _validatorFactory.GetValidator(ActionArgument.Value.GetType());
                if (validator == null)
                {
                    // Console Log (+++)
                    continue;
                }

                ValidationResult result = validator.Validate(ActionArgument.Value);
                if (!result.IsValid)
                {
                    allErrors.AddRange(result.Errors);
                }
            }

            if (allErrors.Count > 0)
            {
                //TODO: Logging validation errors with this specific traceID:
                string traceID = Guid.NewGuid().ToString();

                context.Result = new BadRequestObjectResult(new ValidationError(allErrors, traceID));
            }
        }
    }

}
