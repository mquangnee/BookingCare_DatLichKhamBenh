using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Areas.Patients.Controllers
{
    [Area("Patients")]
    public class HistoryBookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
