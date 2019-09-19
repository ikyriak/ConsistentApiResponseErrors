using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using FluentValidation;
using FluentValidation.Results;
using ConsistentApiResponseErrors.ConsistentErrors;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
        /// Validates Model automatically 
        /// </summary>
        /// <param name="context"></param>
        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //TODO: Add DEBUG level log for the validation result:
            Console.WriteLine("CARE OnActionExecuting: Start validation");

            if (context == null)
            {
                throw new Exception("CARE ValidateModelStateAttribute: The context is null");
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
                        throw new Exception("CARE ValidateModelStateAttribute: The ActionArgument.Value is null");
                    }

                    IValidator validator = _validatorFactory.GetValidator(ActionArgument.Value.GetType());
                    if (validator == null)
                    {
                        //TODO: Add log if no validator is found:
                        Console.WriteLine($"CARE ValidateModelStateAttribute: The validator is null for type {ActionArgument.Value.GetType().ToString()}");
                        continue;
                    }

                    ValidationResult result = validator.Validate(ActionArgument.Value);
                    if (!result.IsValid)
                    {
                        allErrors.AddRange(result.Errors);
                    }

                    //TODO: Add DEBUG level log for the validation result:
                    Console.WriteLine($"CARE ValidateModelStateAttribute: The validation result: {result.IsValid.ToString()} for value \"{ActionArgument.Value.ToString()}\"");
                }
            }

            // When errors exist return a BadRequestObjectResult
            if (allErrors.Count > 0)
            {
                //TODO: Logging validation errors with this specific traceID:
                string traceID = Guid.NewGuid().ToString();

                context.Result = new BadRequestObjectResult(new ValidationError(allErrors, traceID));
            }

            //TODO: Add DEBUG level log for the validation result:
            Console.WriteLine("CARE OnActionExecuting: End validation");
        }
    }

}
