using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class HomeController : Controller
    {
        public HomeController (){}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Index2()
        {
            return View();
        }
        public IActionResult Index3()
        {
            return View();
        }
    }
}
