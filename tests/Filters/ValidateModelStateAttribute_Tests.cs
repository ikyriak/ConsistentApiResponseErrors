using ConsistentApiResponseErrors.Filters;
using ConsistentApiResponseErrors.xUnit.ApplicationServices.Validators;
using FluentValidation;
using System;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using ConsistentApiResponseErrors.xUnit.ApplicationServices.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace ConsistentApiResponseErrors.xUnit.Filters
{
    public class ValidateModelStateAttribute_Tests
    {
        private readonly ILogger<ValidateModelStateAttribute> _logger;

        public ValidateModelStateAttribute_Tests()
        {
            // Show the logs for Debug:
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder
                .AddDebug()
            );
            var factory = serviceCollection.BuildServiceProvider().GetService<ILoggerFactory>();
            _logger = factory.CreateLogger<ValidateModelStateAttribute>();
        }

        /// <summary>
        /// Result to BadRequest: Response in case of a non valid request (based on model state and fluent validator)
        /// </summary>
        [Fact]
        public void SetsResultToBadRequest_IfRequesModelIsInvalid()
        {
            // Arrange
            // Prepare the actionArguments with invalid Id number (0)
            var actionArguments = new Dictionary<string, object>();
            actionArguments.Add("arg1", new RequestModelBasic() { Id = 0, Message = "", CreatedOn = DateTime.Now });

            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(),
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>()
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                actionArguments,
                Mock.Of<Controller>()
            );

            // Prepare the IValidatorFactory with the RequestModelBasicValidator
            var mockValidatorFactory = new Mock<IValidatorFactory>();
            mockValidatorFactory.Setup(vm => vm.GetValidator(typeof(RequestModelBasic))).Returns(new RequestModelBasicValidator());
            var validateModelStateAttribute = new ValidateModelStateAttribute(mockValidatorFactory.Object, _logger);

            // Act
            validateModelStateAttribute.OnActionExecuting(actionExecutingContext);

            // Assert
            Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);

        }

        [Fact]
        public void SetsResultToBadRequest_IfContextModelStateContainErrors()
        {
            // Arrange
            // Prepare the actionArguments with null actionArgument
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("Error_1", "Input string '1.3' is not a valid integer. Path 'userId', line 2, position 17.");
            modelState["Error_1"].RawValue = "1.3";

            modelState.AddModelError("Error_2", "Could not convert string to DateTime: 2018:10. Path 'momentsDate', line 4, position 28.");

            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(),
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>()
            );

            // Prepare the IValidatorFactory with the RequestModelBasicValidator
            var mockValidatorFactory = new Mock<IValidatorFactory>();
            mockValidatorFactory.Setup(vm => vm.GetValidator(typeof(RequestModelBasic))).Returns(new RequestModelBasicValidator());
            var validateModelStateAttribute = new ValidateModelStateAttribute(mockValidatorFactory.Object, _logger);

            // Act
            validateModelStateAttribute.OnActionExecuting(actionExecutingContext);

            // Assert
            Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);
        }


        /// <summary>
        /// Result to Null: Not doing anything, the request is not disturbed
        /// </summary>
        [Fact]
        public void SetsResultToNull_IfRequesModelHasNoValidator()
        {
            // Arrange
            // Prepare the actionArguments using the class RequestModelWithoutValidator:
            var actionArguments = new Dictionary<string, object>();
            actionArguments.Add("arg1", new RequestModelWithoutValidator() { Id = 1, Message = string.Empty, CreatedOn = DateTime.Now });

            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(),
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>()
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                actionArguments,
                Mock.Of<Controller>()
            );

            // Prepare the IValidatorFactory with the RequestModelBasicValidator
            var mockValidatorFactory = new Mock<IValidatorFactory>();
            mockValidatorFactory.Setup(vm => vm.GetValidator(typeof(RequestModelBasic))).Returns(new RequestModelBasicValidator());
            var validateModelStateAttribute = new ValidateModelStateAttribute(mockValidatorFactory.Object, _logger);

            // Act
            validateModelStateAttribute.OnActionExecuting(actionExecutingContext);

            // Assert
            Assert.Null(actionExecutingContext.Result);
        }


        /// <summary>
        /// Throws Exception in unknown cases in order to be safe for unwanted requests
        /// </summary>
        [Fact]
        public void ThrowsException_IfContextIsNull()
        {
            // Arrange
            ActionExecutingContext actionExecutingContext = null;

            // Prepare the IValidatorFactory with the RequestModelBasicValidator
            var mockValidatorFactory = new Mock<IValidatorFactory>();
            mockValidatorFactory.Setup(vm => vm.GetValidator(typeof(RequestModelBasic))).Returns(new RequestModelBasicValidator());
            var validateModelStateAttribute = new ValidateModelStateAttribute(mockValidatorFactory.Object, _logger);

            // Act & Assert (Exception is thrown)
            var ex = Assert.Throws<Exception>(() => validateModelStateAttribute.OnActionExecuting(actionExecutingContext));

            // Assert (Exception message)
            Assert.Equal("ConsistentApiResponseErrors ValidateModelStateAttribute: The context is null", ex.Message);
        }

        [Fact]
        public void ThrowsException_IfContextActionArgumentsIsNul()
        {
            // Arrange
            // Prepare the actionArguments with a null action argument
            var actionArguments = new Dictionary<string, object>();
            actionArguments.Add("arg1", null);

            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(),
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>()
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                actionArguments,
                Mock.Of<Controller>()
            );

            // Prepare the IValidatorFactory with the RequestModelBasicValidator
            var mockValidatorFactory = new Mock<IValidatorFactory>();
            mockValidatorFactory.Setup(vm => vm.GetValidator(typeof(RequestModelBasic))).Returns(new RequestModelBasicValidator());
            var validateModelStateAttribute = new ValidateModelStateAttribute(mockValidatorFactory.Object, _logger);

            // Act & Assert (Exception is thrown)
            var ex = Assert.Throws<Exception>(() => validateModelStateAttribute.OnActionExecuting(actionExecutingContext));

            // Assert (Exception message)
            Assert.Equal("ConsistentApiResponseErrors ValidateModelStateAttribute: The ActionArgument.Value is null", ex.Message);
        }

    }
}
