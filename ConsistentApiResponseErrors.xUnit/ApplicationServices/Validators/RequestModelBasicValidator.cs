using ConsistentApiResponseErrors.xUnit.ApplicationServices.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsistentApiResponseErrors.xUnit.ApplicationServices.Validators
{
    class RequestModelBasicValidator : AbstractValidator<RequestModelBasic>
    {
        public RequestModelBasicValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                    .WithErrorCode("missing_field_value")
                    .WithMessage("The {Id} does not contain value")
                .GreaterThanOrEqualTo(1)
                    .WithErrorCode("bad_format")
                    .WithMessage("{Id} should have a value greatet than zero (0)");

            //RuleFor(x => x.Message)
            //    .NotEmpty()
            //    .MaximumLength(1000)
            //    .WithErrorCode("bad_format")
            //    .WithMessage("{message} should have a value with maximum length of 1000");

            //RuleFor(x => x.MomentsDate)
            //    .NotEmpty()
            //    .WithErrorCode("bad_format")
            //    .WithMessage("The {MomentsDate} does not contain value");

            //RuleFor(x => x.uploadedFiles)
            //    .NotEmpty()
            //    .WithErrorCode("missing_field_value")
            //    .WithMessage("The {uploadedFiles} does not contain value");
        }
    }
}
