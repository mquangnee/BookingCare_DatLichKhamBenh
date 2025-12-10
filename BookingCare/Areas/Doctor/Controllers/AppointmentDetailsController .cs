using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class AppointmentDetailsController : Controller
    {
        public IActionResult Index(int id)
        {
            return View();
        }
    }
}
