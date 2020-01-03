using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Xunit;
using ConsistentApiResponseErrors.Helpers.Enums;

namespace ConsistentApiResponseErrors.Tests.Helpers
{
    public class EnumsExtensions_Tests
    {

        [Theory]
        [InlineData(HttpStatusCode.OK,"OK")]
        [InlineData(HttpStatusCode.BadRequest, "Bad Request")]
        [InlineData(HttpStatusCode.InternalServerError, "Internal Server Error")]
        [InlineData(HttpStatusCode.Created, "Created")]
        [InlineData(HttpStatusCode.ServiceUnavailable, "Service Unavailable")]
        [InlineData(HttpStatusCode.NoContent, "No Content")]
        [InlineData(HttpStatusCode.IMUsed, "IM Used")]
        public void ThrowsException_IfContextIsNull(HttpStatusCode httpStatusCode, string expected)
        {
            // Act:
            string actual = httpStatusCode.ToStringSpaceCamelCase();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
