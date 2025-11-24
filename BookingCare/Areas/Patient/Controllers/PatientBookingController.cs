using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Areas.Patients.Controllers
{
    [Area("Patient")]
    [Authorize(Roles = "Patient")]
    public class PatientBookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
