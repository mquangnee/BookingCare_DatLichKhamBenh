using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Areas.Admin.Controllers
{
    public class MedicineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
