using FluentValidation;

namespace WebApi_5_0.DTOs
{
    public class WeatherForecastRequestValidator : AbstractValidator<WeatherForecastRequest>
    {
        public WeatherForecastRequestValidator()
        {
            RuleFor(x => x.TemperatureC)
                .NotNull()
                    .WithErrorCode("missing_field_value")
                    .WithMessage("The {TemperatureC} does not contain value")
                .GreaterThanOrEqualTo(-20)
                    .WithErrorCode("bad_format")
                    .WithMessage("{TemperatureC} should have a lower value of -20")
                .LessThanOrEqualTo(55)
                    .WithErrorCode("bad_format")
                    .WithMessage("{TemperatureC} should have a greater value of 55");

            RuleFor(x => x.Summary)
                .NotEmpty()
                .MaximumLength(500)
                .WithErrorCode("bad_format")
                .WithMessage("{message} should have a value with maximum length of 1000");
        }
    }
}
