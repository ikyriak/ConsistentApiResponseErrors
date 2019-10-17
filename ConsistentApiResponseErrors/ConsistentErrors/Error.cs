using FluentValidation.Results;
using System;

namespace ConsistentApiResponseErrors.ConsistentErrors
{
    [Serializable]
    public class Error
    {
        public Error()
        {
            Code = string.Empty;
            Field = string.Empty;
            AttemptedValue = null;
            Message = string.Empty;
            HelpURL = string.Empty;
        }

        public Error(ValidationFailure validationFailure)
        {
            Code = validationFailure.ErrorCode;
            Field = validationFailure.PropertyName;
            AttemptedValue = validationFailure.AttemptedValue;
            Message = validationFailure.ErrorMessage;

            // TODO: To be implemented!
            HelpURL = string.Empty;
        }

        public string Code { get; set; }
        public string Field { get; set; }
        public object AttemptedValue { get; set; }
        public string Message { get; set; }
        public string HelpURL { get; set; }
    }
}
