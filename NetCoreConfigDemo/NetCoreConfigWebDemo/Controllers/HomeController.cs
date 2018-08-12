using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace NetCoreConfigWebDemo.Controllers
{
    public class HomeController : Controller
    {
        //private UserInfo _userInfo;
        //public HomeController(IOptions<UserInfo> options)
        //{
        //    _userInfo = options.Value;
        //}
        
        public IActionResult Index()
        {
            //return View(_userInfo);
            return View();
        }
    }
}
