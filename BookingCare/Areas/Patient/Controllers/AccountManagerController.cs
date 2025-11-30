using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BookingCare.Areas.Patients.Controllers
{
    [Area("Patient")]
    [Authorize(Roles = "Patient")]
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
