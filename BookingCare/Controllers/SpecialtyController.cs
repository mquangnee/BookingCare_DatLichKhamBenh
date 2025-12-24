using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Controllers
{
    public class SpecialtyController : Controller
    {
        public IActionResult Detail(int id)
        {
            ViewBag.SpecialtyId = id;
            return View();
        }
    }
}
