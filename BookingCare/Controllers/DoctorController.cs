using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            ViewBag.DoctorId = id;
            return View();
        }
    }
}
