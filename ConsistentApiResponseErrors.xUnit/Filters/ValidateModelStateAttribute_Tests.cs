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

namespace ConsistentApiResponseErrors.xUnit.Filters
{
    public class ValidateModelStateAttribute_Tests
    {
        [Fact]
        public void SetsResultToBadRequest_IfModelIsInvalid()
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
            var validateModelStateAttribute = new ValidateModelStateAttribute(mockValidatorFactory.Object);

            // Act
            validateModelStateAttribute.OnActionExecuting(actionExecutingContext);

            // Assert
            Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);

        }

        [Fact]
        public void SetsResultToNull_IfContextActionArgumentsIsNull()
        {
            // Arrange
            // Prepare the actionArguments with null actionArgument
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
            var validateModelStateAttribute = new ValidateModelStateAttribute(mockValidatorFactory.Object);

            // Act
            validateModelStateAttribute.OnActionExecuting(actionExecutingContext);

            // Assert
            Assert.Null(actionExecutingContext.Result);

        }



    }
}
