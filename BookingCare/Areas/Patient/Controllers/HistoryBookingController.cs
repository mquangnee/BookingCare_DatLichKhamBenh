using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Areas.Patients.Controllers
{
    [Area("Patient")]
    public class HistoryBookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
