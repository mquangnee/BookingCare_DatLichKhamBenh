using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Areas.Admin.Controllers
{
    public class SpecilitesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
