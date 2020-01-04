using ConsistentApiResponseErrors.ConsistentErrors;
using ConsistentApiResponseErrors.Exceptions;
using ConsistentApiResponseErrors.Middlewares;
using ConsistentApiResponseErrors.xUnit.ApplicationServices.DTOs;
using ConsistentApiResponseErrors.xUnit.ApplicationServices.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConsistentApiResponseErrors.Tests.Middlewares
{
    public class ExceptionHandlerMiddleware_Tests
    {
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware_Tests()
        {
            // Show the logs for Debug:
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder
                .AddDebug()
            );
            var factory = serviceCollection.BuildServiceProvider().GetService<ILoggerFactory>();
            _logger = factory.CreateLogger<ExceptionHandlerMiddleware>();
        }



        /// <summary>
        /// Scenario:
        /// An Unexpected Exception is raised.
        /// </summary>
        /// 
        /// Action:
        /// An <see cref="ExceptionError"></see> will be returned with an InternalServerError HTTP status.
        /// 
        [Fact]
        public async Task WhenAnUnexpectedExceptionIsRaised_AnExceptionErrorWillBeReturned_WithInternalServerErrorHttpStatus()
        {            
            // Arrange
            var thrownException = new Exception("Exception with a parameter: {Param1}");
            ExceptionError expectedResponse = new ExceptionError(System.Net.HttpStatusCode.InternalServerError, thrownException, string.Empty);
            var middleware = new ExceptionHandlerMiddleware((innerHttpContext) =>
            {
                throw thrownException;
            }, _logger);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            //Act
            await middleware.Invoke(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var streamText = reader.ReadToEnd();
            var actualResponse = JsonConvert.DeserializeObject<ExceptionError>(streamText);

            //Assert
            Assert.Equal(expectedResponse.StatusCode, actualResponse.StatusCode);
            Assert.Equal(expectedResponse.StatusCode, context.Response.StatusCode);
            Assert.Equal(expectedResponse.StatusMessage, actualResponse.StatusMessage);
            Assert.Equal(expectedResponse.ErrorMessage, actualResponse.ErrorMessage);
        }



        /// <summary>
        /// Scenario:
        /// A Validation Exception based on FluentValidation is raised.
        /// </summary>
        /// 
        /// Action:
        /// An <see cref="ValidationError"></see> will be returned with an BadRequest HTTP status.
        /// 
        [Fact]
        public async Task WhenAFluentValidationExceptionIsRaised_AValidationErrorWillBeReturned_WithBadRequestHttpStatus()
        {
            // Arrange  
            var requestDTO = new RequestModelBasic() { Id = 0, Message = string.Empty, CreatedOn = DateTime.Now };
            
            // Perform a validation using a FluentValidator
            var validator = new RequestModelBasicValidator();
            FluentValidation.Results.ValidationResult validationResult = validator.Validate(requestDTO);
            ValidationException thrownException = new ValidationException(validationResult.Errors, string.Empty);
            
            ValidationError expectedResponse = new ValidationError(validationResult.Errors, string.Empty);
            
            var middleware = new ExceptionHandlerMiddleware((innerHttpContext) =>
            {
                throw thrownException;
            }, _logger);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            //Act
            await middleware.Invoke(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var streamText = reader.ReadToEnd();
            var actualResponse = JsonConvert.DeserializeObject<ValidationError>(streamText);

            //Assert
            Assert.Equal(expectedResponse.StatusCode, actualResponse.StatusCode);
            Assert.Equal(expectedResponse.StatusCode, context.Response.StatusCode);
            Assert.Equal(expectedResponse.StatusMessage, actualResponse.StatusMessage);
            Assert.NotNull(actualResponse.Errors);
            Assert.Equal(expectedResponse.Errors.Count, actualResponse.Errors.Count);

            for (int i = 0; i < actualResponse.Errors.Count; i++)
            {
                Assert.Equal(expectedResponse.Errors[i].Code, actualResponse.Errors[i].Code);
                Assert.Equal(expectedResponse.Errors[i].Message, actualResponse.Errors[i].Message);
                Assert.Equal(expectedResponse.Errors[i].AttemptedValue.ToString(), actualResponse.Errors[i].AttemptedValue.ToString());
                Assert.Equal(expectedResponse.Errors[i].Field, actualResponse.Errors[i].Field);
            }
        }



        /// <summary>
        /// Scenario:
        /// A Validation Exception based on ModelState is raised.
        /// </summary>
        /// 
        /// Action:
        /// An <see cref="ValidationError"></see> will be returned with an BadRequest HTTP status.
        /// 
        [Fact]
        public async Task WhenAModelStateValidationExceptionIsRaised_AValidationErrorWillBeReturned_WithBadRequestHttpStatus()
        {
            // Arrange  
            var requestDTO = new RequestModelBasic() { Id = 0, Message = string.Empty, CreatedOn = DateTime.Now };

            // Perform a validation using ModelState
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("Error_1", "Input string '1.3' is not a valid integer. Path 'userId', line 2, position 17.");
            modelState["Error_1"].RawValue = "1.3";
            modelState.AddModelError("Error_2", "Could not convert string to DateTime: 2018:10. Path 'momentsDate', line 4, position 28.");
            ValidationException thrownException = new ValidationException("bad_request", modelState, string.Empty);

            ValidationError expectedResponse = new ValidationError(thrownException.ValidationFailures, string.Empty);

            var middleware = new ExceptionHandlerMiddleware((innerHttpContext) =>
            {
                throw thrownException;
            }, _logger);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            //Act
            await middleware.Invoke(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var streamText = reader.ReadToEnd();
            var actualResponse = JsonConvert.DeserializeObject<ValidationError>(streamText);

            //Assert
            Assert.Equal(expectedResponse.StatusCode, actualResponse.StatusCode);
            Assert.Equal(expectedResponse.StatusCode, context.Response.StatusCode);
            Assert.Equal(expectedResponse.StatusMessage, actualResponse.StatusMessage);
            Assert.NotNull(actualResponse.Errors);
            Assert.Equal(expectedResponse.Errors.Count, actualResponse.Errors.Count);

            for (int i = 0; i < actualResponse.Errors.Count; i++)
            {
                Assert.Equal(expectedResponse.Errors[i].Code, actualResponse.Errors[i].Code);
                Assert.Equal(expectedResponse.Errors[i].Message, actualResponse.Errors[i].Message);
                Assert.Equal(expectedResponse.Errors[i].AttemptedValue, actualResponse.Errors[i].AttemptedValue);
                Assert.Equal(expectedResponse.Errors[i].Field, actualResponse.Errors[i].Field);
            }
        }



        /// <summary>
        /// Scenario:
        /// An <see cref="ApiBaseException"/> is raised.
        /// </summary>
        /// 
        /// Action:
        /// An <see cref="ExceptionError"></see> will be returned with the provided HTTP status.
        /// 
        [Fact]
        public async Task WhenAnApiBaseExceptionIsRaised_AExceptionErrorWillBeReturned_WithAProvidedHttpStatus()
        {
            // Arrange  
            ApiBaseException thrownException = new ApiBaseException(System.Net.HttpStatusCode.GatewayTimeout, "A Test Exception");
            ExceptionError expectedResponse = new ExceptionError(System.Net.HttpStatusCode.GatewayTimeout, thrownException, string.Empty);

            var middleware = new ExceptionHandlerMiddleware((innerHttpContext) =>
            {
                throw thrownException;
            }, _logger);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            //Act
            await middleware.Invoke(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var streamText = reader.ReadToEnd();
            var actualResponse = JsonConvert.DeserializeObject<ExceptionError>(streamText);

            //Assert
            Assert.Equal(expectedResponse.StatusCode, actualResponse.StatusCode);
            Assert.Equal(expectedResponse.StatusCode, context.Response.StatusCode);
            Assert.Equal(expectedResponse.StatusMessage, actualResponse.StatusMessage);
            Assert.Equal(expectedResponse.ErrorMessage, actualResponse.ErrorMessage);
        }

    }
}
