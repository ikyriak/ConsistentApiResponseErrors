using System;
using System.Collections.Generic;
using System.Text;

namespace ConsistentApiResponseErrors.xUnit.ApplicationServices.DTOs
{
    /// <summary>
    /// A request model without setting a Fluent validator (only the ModelState will be validated)
    /// </summary>
    class RequestModelWithoutValidator
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
