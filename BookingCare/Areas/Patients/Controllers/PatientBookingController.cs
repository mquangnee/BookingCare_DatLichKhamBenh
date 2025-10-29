using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Areas.Patients.Controllers
{
    [Area("Patients")]
    public class PatientBookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
