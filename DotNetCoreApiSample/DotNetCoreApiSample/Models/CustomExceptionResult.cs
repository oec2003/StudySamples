using System;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreApiSample.Models
{
    public class CustomExceptionResult:ObjectResult
    {
        public CustomExceptionResult(int? code, Exception exception)
                : base(new CustomExceptionResultModel(code, exception))
        {
            StatusCode = code;
        }
    }
}
