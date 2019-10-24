# Consistent API Response Errors (CARE) <sup>BETA</sup>

Consistent, structured response bodies on errors are crucial when building a maintainable, usable and predictable API. Consistent API Response Errors (CARE) is an ASP.NET Core middleware that:

- Centralizes the handling of
  - input-validation errors,
  - application exceptions and
  - unhandled exceptions. 
- Simplifies the API controllers by containing only the calls for the appropriate business-login service (without the need of input-validators and try-catch)

If you care about your API consumers, use the CARE middleware and provide usable easy to implement and debug APIs.

## Response Error Types

| Error Type             | Description                                                  |
| ---------------------- | ------------------------------------------------------------ |
| Validation Errors      | Providing details about the validation errors of the input request. [FluentValidation](https://fluentvalidation.net/) is required to defining the validation rules. |
| Application Exceptions | User-defined exceptions thrown to provide details about application-specific or business logic issues. |
| Unhandled Exceptions   | Thrown when a non-recoverable error has occurred. These are common exceptions that are thrown by the .NET Common Language Runtime. |

## Response Formats

Error response messages provide additional information that can be used to debug the error as well as providing user-friendly feedback.

### The CARE Response Formats

#### General Response Error Format

The following table presents the fields of the response for application and unhandled exceptions:

| Field         | Type    | Description                                                  |
| ------------- | ------- | ------------------------------------------------------------ |
| statusCode    | Integer | The HTTP status code. The `StatusCode` and `StatusMessage` fields would come handy when someone is testing the API via browser. |
| statusMessage | String  | The HTTP status message.                                     |
| traceId       | String  | A Unique Id that can be used to trace the error in your logging system for more information (e.g. to see the Stack-Trace). |
| errorMessage  | String  | A short human-readable string that describes the error.      |

An example of the general error format for application and unhandled exceptions:

```json
HTTP/1.1 503 Service Unavailable
Content-Type: application/problem+json

{
    "statusCode": 503,
    "statusMessage": "Service Unavailable",
    "errorMessage": "No modifications were performed to the entity",
    "traceId": "126a2854-9b27-4284-9498-3210091df834",
}
```

#### Validation Response Errors Format

In a multiple validation errors scenario, there is the possibility that the consumer has more than one validation error in their request.

The following table presents the fields of the response for validations errors:

| Field         | Type    | Description                                                  |
| ------------- | ------- | ------------------------------------------------------------ |
| statusCode   | Integer | The HTTP status code. The `StatusCode` and `StatusMessage` fields would come handy when someone is testing the API via browser. |
| statusMessage | String  | The HTTP status message.                                     |
| traceId      | String  | A Unique Id that can be used to trace the error in your logging system for more information (e.g. to see the StackTrace). |
| errors       | Array   | An array containing multiple validation errors |
| &#9492; code | String | A unique error code describing the specific error case. Using a numeric error code is the most common approach. Personally a suggest the use of string based error codes as they are easy to read. |
| &#9492; field | String | The name of the field which has this error (as in the incoming request). |
| &#9492;&nbsp;attemptedValue | Object | Contains the attempted-original value from the request. This is very useful in cases where an array of same resources are accepted. The attempted-original value in the response helps the consumer to track which resource in the request had the error. |
| &#9492; message | String | A short human-readable string that describes the error. |
| &#9492; helpURL | String | A URL to a page that describes this particular error in more detail. |

An example of the validation response error format:

```json
HTTP/1.1 400 Bad Request
Content-Type: application/json; charset=utf-8

{
    "statusCode": 400,
    "statusMessage": "Bad Request",
    "traceId": "107f18b1-6585-4f08-8359-df12971e33ad",
    "errors": [
        {
            "code": "bad_format",
            "field": "email",
            "attemptedValue": "test1test.gr",
            "message": "{email} is not in correct format",
            "helpURL": ""
        },
        {
            "code": "not_empty",
            "field": "password",
            "attemptedValue": "",
            "message": "'password' must not be empty.",
            "helpURL": ""
        }
    ]
}
```

### The Problem Details Response Format (RFC7807)

[RFC7807](https://tools.ietf.org/html/rfc7807) defines a "problem detail" as a way to carry machine-readable details of errors in a HTTP response to avoid the need to define new error response formats for HTTP APIs.

`This format is currently not supported by CARE middleware.`

An example of the RFC7807 Problem Details Response: 

```json
HTTP/1.1 400 Bad Request
Content-Type: application/problem+json
Content-Language: en

{
	"type": "https://example.net/validation-error",
	"title": "Your request parameters didn't validate.",
	"invalid-params": [ {
		"name": "age",
		"reason": "must be a positive integer"
	},
	{
		"name": "color",
		"reason": "must be 'green', 'red' or 'blue'"
    } ]
}
```

## NuGet package

CARE middleware is available as a NuGet package. The NuGet package can be accessed [here](https://github.com/ikyriak/ConsistentApiResponseErrors).

## Configuration

### Using CARE for Unhandled Exceptions

 In the `Configure` method, add the following:

```c#
// CARE for all Unhandled exceptions
app.UseMiddleware<ConsistentApiResponseErrors.Middlewares.ExceptionHandlerMiddleware>();
```

### Using CARE for Application Exceptions

Initially perform the previous step in order to use the Middleware.

CARE can be used for your Application Exceptions by changing them in order to implement  the `ApiBaseException`, which requires the definition of the following parameters:

- HTTP Status Code
- HTTP Status Message
- Error Message

For example, if one would throw an exception when an entity is not found, then you'd start by implementing the `ApiBaseException` class for that exception and define the aforementioned parameters, appropriately:

```c#
using System;
using ConsistentApiResponseErrors.Exceptions;

namespace ExampleProject.Core.Application.Exceptions
{
    [Serializable]
    public class EntityNotFoundException : ApiBaseException
    {
        private const int _httpStatusCode = 404;
        private const string _httpStatusMessage = "Not Found";
        private const string _DefaultErrorMessage = "Entity not found";

        public EntityNotFoundException() :
            base(_httpStatusCode, _httpStatusMessage, _DefaultErrorMessage)
        {

        }

        public EntityNotFoundException(string message)
           : base(_httpStatusCode, _httpStatusMessage, message)
        {

        }
    }
}
```

### Using CARE for Validation Errors

#### Create the Fluent-Validators for all request DTOs.

For example, add validators for the properties of the `ExampleRequestModel` DTO class. More details about FluentValidation visit the project's [website](https://fluentvalidation.net/).

```c#
using FluentValidation;
using ExampleProject.Core.Application.Services.DTOs;

namespace ExampleProject.Core.Application.Services.Validators
{
    public class ExampleRequestModelValidator : AbstractValidator<ExampleRequestModel>
    {
        public MomentRequestModelValidator()
        {
            RuleFor(x => x.UserId)
                .NotNull()
                    .WithErrorCode("missing_field_value")
                    .WithMessage("The {UserId} does not contain value")
                .GreaterThanOrEqualTo(1)
                    .WithErrorCode("bad_format")
                    .WithMessage("{UserId} should have a value greatet than zero (0)");

            RuleFor(x => x.Message)
                .NotEmpty()
                .MaximumLength(1000)
                .WithErrorCode("bad_format")
                .WithMessage("{message} should have a value with maximum length of 1000");
        }
    }
}
```

#### Setup using statements

 In your `Startup` class, add a using statements:

```c#
using ConsistentApiResponseErrors.Filters;
using FluentValidation.AspNetCore;
```

#### Configure the MVC Service

In your `ConfigureServices` class, register all validators from assembly by setting the `YOUR_VALIDATOR` with one them. In addition, suppress the .NET default validation filters.

```c#
// Register all validators within a particular assembly:
services.AddMvc(options =>
{
    // Use CARE model validator to reduce code duplication
    options.Filters.Add<ValidateModelStateAttribute>();
})
.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<YOUR_VALIDATOR>());

// Configure the default API behavour by setting up the MVC application to suppress validation filters in order to be handled by the `ConsistentApiResponseErrors.Filters.ValidateModelStateAttribute`
services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
```

