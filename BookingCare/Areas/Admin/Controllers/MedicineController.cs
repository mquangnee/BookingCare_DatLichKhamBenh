using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MedicineController : Controller
    {
        public MedicineController() {}   

        //Hiển thị trang quản lý thuốc
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
