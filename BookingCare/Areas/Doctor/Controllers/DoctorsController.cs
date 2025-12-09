using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Areas.Doctors.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles ="Doctor")]
    public class DoctorsController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int id)
        {
            ViewBag.AppointmentId = id;
            return View();
        }

        public IActionResult TraKetQuaKhamBenh(int id)
        {
            ViewBag.AppointmentId = id;
            return View();
        }


        public IActionResult ReturnResult(int id)
        {
            ViewBag.AppointmentId = id;
            return View();
        }
    }
}
