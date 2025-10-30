using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Areas.Patients.Controllers
{
    [Area("Patients")]
    public class AccountManagerController : Controller
    {
    
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
    }
}
