using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Areas.Admin.Controllers
{
    public class UserManagerController : Controller
    {
        public IActionResult PatientManager()
        {
            return View();
        }
        public IActionResult DoctorManager()
        {
            return View();
        }
    }
}
