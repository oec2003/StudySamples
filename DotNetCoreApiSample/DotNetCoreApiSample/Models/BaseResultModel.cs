using System;
namespace DotNetCoreApiSample.Models
{
    public class BaseResultModel
    {
        public BaseResultModel(int? code = null, string message = null,
            object result = null, ReturnStatus returnStatus = ReturnStatus.Success)
        {
            this.Code = code;
            this.Result = result;
            this.Message = message;
            this.ReturnStatus = returnStatus;
        }
        public int? Code { get; set; }

        public string Message { get; set; }

        public object Result { get; set; }

        public ReturnStatus ReturnStatus { get; set; }
    }
    public enum ReturnStatus
    {
        Success = 1,
        Fail = 0,
        ConfirmIsContinue = 2,
        Error = 3
    }
}
