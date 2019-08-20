using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DotNetCoreApiSample.Models
{
    public class ValidationFailedResult: ObjectResult
    {

        public ValidationFailedResult(ModelStateDictionary modelState)
              : base(new ValidationFailedResultModel(modelState))
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity;
        }
    }
}
