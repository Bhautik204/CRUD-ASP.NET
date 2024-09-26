using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Controllers
{
    public class UserLogin : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
