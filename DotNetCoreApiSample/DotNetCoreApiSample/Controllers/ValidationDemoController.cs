using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreApiSample.Controllers
{
    [Route("api/[controller]")]
    public class ValidationDemoController : Controller
    {
        [HttpPost]
       public IActionResult AddUser([FromBody]User user)
       {
            //业务代码
            return Ok();
       }
    }

    public class User
    {
        [Required(ErrorMessage = "用户Code不能为空")]
        public string Code { get; set; }
        [Required(ErrorMessage = "用户名称不能为空")]
        public string Name { get; set; }
        [Required(ErrorMessage = "用户年龄不能为空")]
        [Range(1, 100, ErrorMessage = "年龄必须介于1~100之间")]
        public int Age { get; set; }
        public string Address { get; set; }
    }
}
