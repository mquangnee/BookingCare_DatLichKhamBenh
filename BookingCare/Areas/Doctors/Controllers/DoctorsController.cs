using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Areas.Doctors.Controllers
{
    [Area("Doctors")]
    public class DoctorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult TraKetQuaKhamBenh()
        {
            return View();
        }
        public IActionResult Detail()
        {
            return View();
        }
        public IActionResult ReturnResult()
        {
            return View();
        }
    }
}
