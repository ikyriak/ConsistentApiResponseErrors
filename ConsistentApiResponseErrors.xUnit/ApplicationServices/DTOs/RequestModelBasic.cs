using System;
using System.Collections.Generic;
using System.Text;

namespace ConsistentApiResponseErrors.xUnit.ApplicationServices.DTOs
{
    /// <summary>
    /// A basic request model with several types of input
    /// </summary>
    class RequestModelBasic
    {
        public int Id { get; set; }
        //public Guid guId { get; set; }
        //public decimal Price { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
        //public DateTime? DeletedOn { get; set; }
        //public long? FileSize { get; set; }
        //public List<string> ListNames { get; set; }
    }
}
